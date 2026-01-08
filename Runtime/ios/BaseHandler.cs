using System;
using WebSocketClientPackage.Runtime.managers;
using WebSocketClientPackage.Runtime.protocols;
using WebSocketClientPackage.Runtime.utils;
using UnityEngine;

namespace WebSocketClientPackage.Runtime.ios
{
    public class BaseHandler
    {
        private BaseHandler()
        {
        }

        /// <summary>
        ///     Xử lý request từ client
        /// </summary>
        /// <param name="kingSocket"></param>
        /// <param name="buffer"></param>
        public void HandleResponeHandler(byte[] buffer)
        {
            KingMessage kMsg = new KingMessage(buffer);

            var controllerId = 0;
            var requestId = 0;

            try
            {
                controllerId = kMsg.ReadInt32();
            }
            catch (Exception e)
            {
                controllerId = 0;
                Debug.LogWarning(
                    $"[BaseHandler|HandleRequestHandler] Error reading controllerId: {e.Message}");
            }

            if (controllerId == StaticController.PING_CONTROLLER)
                requestId = StaticRequest.PING;
            else
                try
                {
                    requestId = kMsg.ReadInt32();
                }
                catch (Exception e)
                {
                    requestId = 0;
                    Debug.LogWarning(
                        $"[BaseHandler|HandleRequestHandler] Error reading requestId: {e.Message}");
                }

            kMsg.SetControllerId(controllerId);
            kMsg.SetRequestId(requestId);

            // Debug.Log("--------------------- Receive -------------------");
            // Debug.Log($"controllerId: {controllerId}");
            // Debug.Log($"requestId: {requestId}");
            // Debug.Log("-------------------------------------------------");

            HandlerManager.Instance.HandleServerResponse(kMsg);
        }

        /// <summary>
        ///     Xử lý khi socket đóng
        /// </summary>
        /// <param name="kingSocket"></param>
        public void HandleSocketCloseHandler()
        {
            
        }

        /// <summary>
        ///     Xử lý khi socket kết nối
        /// </summary>
        /// <param name="kingSocket"></param>
        public void HandleSocketConnectHandler()
        {
        }

        #region Singleton Pattern

        private static readonly Lazy<BaseHandler> _instance = new(() => new BaseHandler());
        public static BaseHandler Instance => _instance.Value;

        #endregion
    }
}