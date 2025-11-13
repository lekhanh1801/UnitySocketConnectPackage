// Assets/SocketClient/IConnection.cs

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WebSocketClientPackage.WebSocketClientPackage.Runtime.connections;

namespace WebSocketClientPackage.Runtime.connections
{
    public abstract class Connection<TSocket, TSocketState> : IConnection               
        where TSocket : class
    {
        #region Events
        public virtual event Action<string> OnMessageReceived;
        public virtual event Action<byte[]> OnBinaryMessageReceived;
        public virtual event Action OnConnected;
        public virtual event Action OnDisconnected;
        public virtual event Action<string> OnError;
        public virtual event Action<int> OnReconnecting;
        #endregion

        #region Private Fields

        protected TSocket _socket;
        protected CancellationTokenSource _cancellationTokenSource;
        protected readonly Uri _serverUri;
        protected bool _isConnecting;
        protected bool _isDisposed;
        protected TSocketState _previousState;

        // Reconnection settings
        protected int _reconnectAttempts;
        protected int _maxReconnectAttempts = 5;
        protected int _reconnectDelayMs = 2000;
        protected bool _autoReconnect = true;

        // Message queue for when connection is not ready
        protected Queue<string> _messageQueue = new();
        protected Queue<byte[]> _binaryMessageQueue = new();
        protected readonly object _queueLock = new();

        // Configuration
        protected readonly TimeSpan _keepAliveInterval = TimeSpan.FromSeconds(30);
        protected readonly int _receiveBufferSize = 4096;

        #endregion

        #region Public Properties

        public string ServerUrl => _serverUri?.ToString();
        public bool IsConnected = false;

        public int QueuedMessageCount
        {
            get
            {
                lock (_queueLock)
                {
                    return _messageQueue.Count + _binaryMessageQueue.Count;
                }
            }
        }

        public int ReconnectAttempts => _reconnectAttempts;

        public bool AutoReconnect
        {
            get => _autoReconnect;
            set => _autoReconnect = value;
        }

        public int MaxReconnectAttempts
        {
            get => _maxReconnectAttempts;
            set => _maxReconnectAttempts = Math.Max(0, value);
        }

        public int ReconnectDelayMs
        {
            get => _reconnectDelayMs;
            set => _reconnectDelayMs = Math.Max(100, value);
        }

        #endregion

        #region Constructor

        /// <summary>
        ///     Initializes a new instance of the GameWebSocketClient class
        /// </summary>
        /// <param name="url">WebSocket server URL (ws:// or wss://)</param>
        /// <exception cref="ArgumentException">Thrown when URL is invalid</exception>
        public Connection(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("URL cannot be null or empty", nameof(url));

            if (!url.StartsWith("ws://") && !url.StartsWith("wss://"))
                throw new ArgumentException("URL must start with ws:// or wss://", nameof(url));

            try
            {
                _serverUri = new Uri(url);
            }
            catch (UriFormatException ex)
            {
                throw new ArgumentException("Invalid URL format", nameof(url), ex);
            }

            InitializeSocket();
        }

        public Connection()
        {
            InitializeSocket();
        }

        #endregion

        #region Public Methods

        public async Task ConnectAsync()
        {
        }

        /// <summary>
        ///     Sends a text message to the server asynchronously
        /// </summary>
        /// <param name="message">The text message to send</param>
        /// <returns>Task representing the send operation</returns>
        public async Task SendAsync(string message)
        {
        }

        /// <summary>
        ///     Sends a binary message to the server asynchronously
        /// </summary>
        /// <param name="data">The binary data to send</param>
        /// <returns>Task representing the send operation</returns>
        public async Task SendAsync(byte[] data)
        {
        }

        /// <summary>
        ///     Closes the WebSocket connection asynchronously
        /// </summary>
        /// <param name="closeMessage">Optional close message</param>
        /// <returns>Task representing the close operation</returns>
        public async Task CloseAsync(string closeMessage = "Client disconnecting")
        {
        }

        /// <summary>
        ///     Clears all queued messages that haven't been sent yet
        /// </summary>
        public void ClearQueuedMessages()
        {
            lock (_queueLock)
            {
                _messageQueue.Clear();
                _binaryMessageQueue.Clear();
                Debug.Log("[WebSocket] Cleared queued messages");
            }
        }

        /// <summary>
        ///     Disposes the WebSocket client and releases all resources
        /// </summary>
        public void Dispose()
        {
        }

        #endregion

        #region Private Methods

        protected virtual async Task InitializeSocket()
        {
        }

        protected virtual async Task ReceiveLoop()
        {
        }

        protected async Task ProcessMessageQueue()
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

        protected async Task AttemptReconnection()
        {
            if (!_autoReconnect || _reconnectAttempts >= _maxReconnectAttempts)
                return;

            _reconnectAttempts++;
            OnReconnecting?.Invoke(_reconnectAttempts);

            Debug.Log($"[WebSocket] Attempting reconnection ({_reconnectAttempts}/{_maxReconnectAttempts})...");

            // // Exponential backoff
            // int delay = _reconnectDelayMs * (int)Math.Pow(2, _reconnectAttempts - 1);
            // await Task.Delay(delay, cancellationTokenSource.Token);
            //
            // if (!cancellationTokenSource.Token.IsCancellationRequested)
            // {
            //     // Reinitialize WebSocket and try to connect again
            //     _socket?.Dispose();
            //     InitializeWebSocket();
            //     _ = ConnectAsync();
            // }
        }

        protected void UpdateState()
        {
        }

        #endregion

        #region Protected Methods

        protected void ReceivedMessaged(string message)
        {
            OnMessageReceived?.Invoke(message);
        }

        protected void BinaryReceivedMessaged(byte[] message)
        {
            OnBinaryMessageReceived?.Invoke(message);
        }

        protected void Connected()
        {
            OnConnected?.Invoke();
        }

        protected void Disconnected()
        {
            OnDisconnected?.Invoke();
        }

        protected void Error(string errorMessage)
        {
            OnError?.Invoke(errorMessage);
        }

        protected void Reconnecting(int reconnectAttempts)
        {
            OnReconnecting?.Invoke(reconnectAttempts);
        }

        protected bool TryParseMessage(List<byte> buffer, out byte[] message)
        {
            message = null;
            if (buffer.Count <= 0) return false; 
            int length = BitConverter.ToInt32(buffer.GetRange(0, 0).ToArray(), 0);
            if (buffer.Count < length) return false; // chưa đủ payload
            message = buffer.GetRange(0, length).ToArray();
            buffer.RemoveRange(0, length);
            return true;
        }

        #endregion
    }
}