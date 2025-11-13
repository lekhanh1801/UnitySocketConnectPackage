using System;
using WebSocketClientPackage.Runtime.protocols;
using UnityEngine;
using WebSocketClientPackage.Runtime.handlers;
using WebSocketClientPackage.Runtime.helpers;
using WebSocketClientPackage.Runtime.managers;
using WebSocketClientPackage.Runtime.utils;

namespace WebSocketClientPackage.Runtime.ios
{
    public class WebSocketHandler
    {
        private static WebSocketHandler _instance;
        public static WebSocketHandler Instance => _instance ??= new WebSocketHandler();

        /// <summary>
        /// Khởi tạo SocketHandler
        /// </summary>
        private WebSocketHandler()
        {
        }

        /// <summary>
        /// Xử lý event 'connected' của socket
        /// </summary>
        public void OnConnected()
        {
            // if (MatchingManager.Instance.IsSuccessMatch)
            // {
            //     return;
            // }
            //
            uint timeStamp = (uint) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            KingMessage kMsg = new KingMessage();
            kMsg.SetDirection(StaticController.SYSTEM_CONTROLLER, 19);
            kMsg.WriteUInt32(timeStamp);
            string sign = "longbutshort" + timeStamp + 33127;
            kMsg.WriteString("");
            kMsg.WriteString(HashUtils.Sha256(sign));
            KingHelper.Send(kMsg);
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