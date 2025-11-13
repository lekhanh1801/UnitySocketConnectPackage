using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

using UnityEditor.PackageManager;
using UnityEngine;

namespace WebSocketClientPackage.Runtime.ios
{
    public class SocketHandler
    {
        private static SocketHandler _instance;
        public static SocketHandler Instance => _instance ??= new SocketHandler();

        /// <summary>
        /// Khởi tạo SocketHandler
        /// </summary>
        private SocketHandler()
        {
        }

        /// <summary>
        /// Xử lý event 'connected' của socket
        /// </summary>
        /// <param name="socket">TCP socket</param>
        public void OnConnected()
        {
        }

        /// <summary>
        /// Xử lý event 'connected' của socket
        /// </summary>
        /// <param name="socket">TCP socket</param>
        public void OnReConnect(int State)
        {
        }

        /// <summary>
        /// Xử lý event 'disconnected' của socket
        /// </summary>
        /// <param name="kingSocket">KingSocket instance</param>
        /// <param name="hadError">Có lỗi hay không</param>
        public void OnDisconnected()
        {
        }

        /// <summary>
        /// Xử lý event 'message' của socket
        /// </summary>
        /// <param name="kingSocket">KingSocket instance</param>
        /// <param name="buffer">Dữ liệu nhận được</param>
        public void OnMessage(byte[] buffer)
        {
            if (buffer == null || buffer.Length <= 0) {
                Debug.LogWarning("buffer is invalid!!!");
            }

            try
            {
                BaseHandler.Instance.HandleResponeHandler(buffer);
            }
            catch (Exception ex)
            {
                var errorMessage = $"[WebSocketHandler] OnMessage Failed : {ex.Message}";
                Debug.LogError(errorMessage);
                throw;
            }
        }

        /// <summary>
        /// Xử lý event 'error' của socket
        /// </summary>
        /// <param name="err">Exception</param>
        public void OnError(string err)
        {
        }

        /// <summary>
        /// Bắt đầu nhận dữ liệu từ TCP socket
        /// </summary>
        /// <param name="kingSocket">KingSocket instance</param>
        private void StartReceiving()
        {
        }
    }
    
}