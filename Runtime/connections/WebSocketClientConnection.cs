using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketClientPackage.Runtime.connections;
using UnityEngine;

namespace WebSocketClientPackage.Runtime.Connections
{
    /// <summary>
    ///     A robust WebSocket client for Unity game development
    ///     Supports automatic reconnection, message queuing, and JSON serialization
    /// </summary>
    public class WebSocketClientConnection : Connection<ClientWebSocket, WebSocketState>, IDisposable
    {
        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the GameWebSocketClient class
        /// </summary>
        /// <param name="url">WebSocket server URL (ws:// or wss://)</param>
        /// <exception cref="ArgumentException">Thrown when URL is invalid</exception>
        public WebSocketClientConnection(string url) : base(url)
        {
        }

        #endregion

        #region Events

        public event Action<WebSocketState> OnStateChanged;

        #endregion

        #region Public Properties

        public WebSocketState State => _socket?.State ?? WebSocketState.None;
        public bool IsConnected => _socket?.State == WebSocketState.Open;

        #endregion

        #region Public Methods

        public async Task ConnectAsync()
        {
            if (_isConnecting || IsConnected || _isDisposed)
                return;

            _isConnecting = true;
            _reconnectAttempts = 0;

            try
            {
                // Debug.Log($"[WebSocket] Connecting to {_serverUri}...");

                await _socket.ConnectAsync(_serverUri, _cancellationTokenSource.Token);

                // Debug.Log($"[WebSocket] Connected to {_serverUri}");
                Connected();
                UpdateState();

                _ = Task.Run(ReceiveLoop, _cancellationTokenSource.Token);

                _ = Task.Run(ProcessMessageQueue, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("[WebSocket] Connection cancelled");
            }
            catch (Exception ex)
            {
                var errorMessage = $"[WebSocket] Connection failed: {ex.Message}";
                Debug.LogError(errorMessage);
                Error(errorMessage);

                if (_autoReconnect && _reconnectAttempts < _maxReconnectAttempts) await AttemptReconnection();
            }
            finally
            {
                _isConnecting = false;
            }
        }

        /// <summary>
        ///     Sends a text message to the server asynchronously
        /// </summary>
        /// <param name="message">The text message to send</param>
        /// <returns>Task representing the send operation</returns>
        public async Task SendAsync(string message)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(WebSocketClientConnection));

            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("Message cannot be null or empty", nameof(message));

            if (!IsConnected)
            {
                lock (_queueLock)
                {
                    _messageQueue.Enqueue(message);
                }

                return;
            }

            try
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                await _socket.SendAsync(
                    new ArraySegment<byte>(buffer),
                    WebSocketMessageType.Text,
                    true,
                    _cancellationTokenSource.Token
                );

                Debug.Log($"[WebSocket] Sent text message: {message}");
            }
            catch (Exception ex)
            {
                var errorMessage = $"[WebSocket] Send failed: {ex.Message}";
                Debug.LogError(errorMessage);
                Error(errorMessage);

                // Queue the failed message for retry
                lock (_queueLock)
                {
                    _messageQueue.Enqueue(message);
                }
            }
        }

        /// <summary>
        ///     Sends a binary message to the server asynchronously
        /// </summary>
        /// <param name="data">The binary data to send</param>
        /// <returns>Task representing the send operation</returns>
        public async Task SendAsync(byte[] data)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(WebSocketClientConnection));

            if (data == null || data.Length == 0)
                throw new ArgumentException("Data cannot be null or empty", nameof(data));

            if (!IsConnected)
            {
                lock (_queueLock)
                {
                    _binaryMessageQueue.Enqueue(data);
                }
                return;
            }

            try
            {
                await _socket.SendAsync(
                    new ArraySegment<byte>(data),
                    WebSocketMessageType.Binary,
                    true,
                    _cancellationTokenSource.Token
                );

                Debug.Log($"[WebSocket] Sent binary message: {data.Length} bytes");
            }
            catch (Exception ex)
            {
                var errorMessage = $"[WebSocket] Binary send failed: {ex.Message}";
                Debug.LogError(errorMessage);
                Error(errorMessage);

                lock (_queueLock)
                {
                    _binaryMessageQueue.Enqueue(data);
                }
            }
        }

        /// <summary>
        ///     Closes the WebSocket connection asynchronously
        /// </summary>
        /// <param name="closeMessage">Optional close message</param>
        /// <returns>Task representing the close operation</returns>
        public async Task CloseAsync(string closeMessage = "Client disconnecting")
        {
            if (_isDisposed || _socket == null)
                return;

            try
            {
                if (IsConnected)
                {
                    await _socket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        closeMessage,
                        _cancellationTokenSource.Token
                    );

                    Debug.Log("[WebSocket] Connection closed gracefully");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[WebSocket] Close warning: {ex.Message}");
            }
            finally
            {
                Disconnected();
                UpdateState();
            }
        }

        /// <summary>
        ///     Disposes the WebSocket client and releases all resources
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            try
            {
                // Cancel any ongoing operations
                _cancellationTokenSource?.Cancel();

                // Close connection if still open
                if (_socket?.State == WebSocketState.Open)
                    _ = _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disposing", CancellationToken.None);

                _socket?.Dispose();
                _cancellationTokenSource?.Dispose();

                lock (_queueLock)
                {
                    _messageQueue.Clear();
                    _binaryMessageQueue.Clear();
                }

                Debug.Log("[WebSocket] Client disposed");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[WebSocket] Dispose error: {ex.Message}");
            }
        }

        #endregion

        #region Private Methods

        protected override async Task InitializeSocket()
        {
            _socket = new ClientWebSocket();
            _cancellationTokenSource = new CancellationTokenSource();
            _previousState = WebSocketState.None;

            // Configure WebSocket options
            _socket.Options.KeepAliveInterval = _keepAliveInterval;
            await ConnectAsync();

            Debug.Log($"[WebSocket] Client initialized for {_serverUri}");
        }

        protected override async Task ReceiveLoop()
        {
            var buffer = new byte[_receiveBufferSize];
            var messageBuffer = new List<byte>();

            try
            {
                Debug.Log($"IsConnected: {IsConnected}");
                Debug.Log($"!_cancellationTokenSource.Token.IsCancellationRequested: {!_cancellationTokenSource.Token.IsCancellationRequested}");

                while (IsConnected && !_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var result = await _socket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        _cancellationTokenSource.Token
                    );

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Debug.Log("[WebSocket] Server requested closure");
                        // await CloseAsync("Server closed connection");
                        break;
                    }

                    // Add received data to message buffer
                    messageBuffer.AddRange(new ArraySegment<byte>(buffer, 0, result.Count));

                    // If this is the final message, process it
                    if (result.EndOfMessage)
                    {
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            var message = Encoding.UTF8.GetString(messageBuffer.ToArray());
                            Debug.Log($"[WebSocket] Received text message: {message}");
                            ReceivedMessaged(message);
                        }
                        else if (result.MessageType == WebSocketMessageType.Binary)
                        {
                            var data = messageBuffer.ToArray();
                            Debug.Log($"[WebSocket] Received binary message: {data.Length} bytes");
                            BinaryReceivedMessaged(data);
                        }

                        messageBuffer.Clear();
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
            }
            catch (Exception ex)
            {
                var errorMessage = $"[WebSocket] Receive error: {ex.Message}";
                Debug.LogError(errorMessage);
                Error(errorMessage);

                if (!_cancellationTokenSource.Token.IsCancellationRequested && _autoReconnect)
                    await AttemptReconnection();
            }
            finally
            {
                if (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    Disconnected();
                    UpdateState();
                }
            }
        }

        private async Task ProcessMessageQueue()
        {
            while (IsConnected && !_cancellationTokenSource.Token.IsCancellationRequested)
                try
                {
                    string textMessage = null;
                    byte[] binaryMessage = null;

                    lock (_queueLock)
                    {
                        if (_messageQueue.Count > 0)
                            textMessage = _messageQueue.Dequeue();
                        else if (_binaryMessageQueue.Count > 0)
                            binaryMessage = _binaryMessageQueue.Dequeue();
                    }

                    if (textMessage != null)
                        await SendAsync(textMessage);
                    else if (binaryMessage != null)
                        await SendAsync(binaryMessage);
                    else
                        // No messages in queue, wait a bit
                        await Task.Delay(100, _cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[WebSocket] Queue processing error: {ex.Message}");
                    await Task.Delay(1000, _cancellationTokenSource.Token);
                }
        }

        private async Task AttemptReconnection()
        {
            if (!_autoReconnect || _reconnectAttempts >= _maxReconnectAttempts)
                return;

            _reconnectAttempts++;
            Reconnecting(_reconnectAttempts);

            Debug.Log($"[WebSocket] Attempting reconnection ({_reconnectAttempts}/{_maxReconnectAttempts})...");

            // Exponential backoff
            var delay = _reconnectDelayMs * (int)Math.Pow(2, _reconnectAttempts - 1);
            await Task.Delay(delay, _cancellationTokenSource.Token);

            if (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                // Reinitialize WebSocket and try to connect again
                _socket?.Dispose();
                InitializeSocket();
                _ = ConnectAsync();
            }
        }

        private void UpdateState()
        {
            var currentState = State;
            if (_previousState != currentState)
            {
                _previousState = currentState;
                OnStateChanged?.Invoke(currentState);
                Debug.Log($"[WebSocket] State changed to: {currentState}");
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        ///     Gets the current connection status as a descriptive string
        /// </summary>
        public string GetStatusString()
        {
            return State switch
            {
                WebSocketState.None => "Not initialized",
                WebSocketState.Connecting => "Connecting...",
                WebSocketState.Open => $"Connected to {ServerUrl}",
                WebSocketState.CloseSent => "Closing connection...",
                WebSocketState.CloseReceived => "Server closing connection",
                WebSocketState.Closed => "Disconnected",
                _ => "Unknown state"
            };
        }

        /// <summary>
        ///     Pings the server by sending a small message (if supported by server protocol)
        /// </summary>
        public async Task SendPingAsync()
        {
            if (IsConnected)
                try
                {
                    var pingMessage = $"ping|{DateTime.UtcNow.Ticks}";
                    await SendAsync(pingMessage);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[WebSocket] Ping failed: {ex.Message}");
                }
        }

        #endregion
    }
}