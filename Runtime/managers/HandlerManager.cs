using System;
using System.Collections.Generic;
using WebSocketClientPackage.Runtime.handlers.base_handlers;
using WebSocketClientPackage.Runtime.protocols;
using WebSocketClientPackage.Runtime.utils;
using UnityEngine;
using WebSocketClientPackage.Runtime.handlers;

namespace WebSocketClientPackage.Runtime.managers
{
    public class HandlerManager
    {
        private readonly Dictionary<int, IServerResponeHandler> _responseHandlers;
        private readonly Dictionary<int, IServerResponeHandler> _systemHandlers;

        /// <summary>
        ///     Khởi tạo HandlerManager
        /// </summary>
        private HandlerManager()
        {
            _responseHandlers = new Dictionary<int, IServerResponeHandler>();
            _systemHandlers = new Dictionary<int, IServerResponeHandler>();
        }

        /// <summary>
        ///     Khởi tạo handler request
        /// </summary>
        public void Init()
        {
            // AddSystemResponseHandler<MatchingCompleteHandler>(7);
            // AddClientRequestHandler<PingHandler>(StaticRequest.PING);
            // AddClientRequestHandler<MasterMatchingHandler>(StaticRequest.MASTER_MATCHING);
        }

        /// <summary>
        ///     Thêm client request handler
        /// </summary>
        /// <param name="cmd">Command string</param>
        /// <param name="handlerType">Type của handler</param>
        public void AddServerResponseHandler(int cmd, Type handlerType)
        {
            if (Activator.CreateInstance(handlerType) is IServerResponeHandler handler) _responseHandlers[cmd] = handler;
        }

        /// <summary>
        ///     Thêm server response handler (generic version)
        /// </summary>
        /// <typeparam name="T">Handler type</typeparam>
        /// <param name="cmd">Command string</param>
        public void AddServerResponseHandler<T>(int cmd) where T : IServerResponeHandler, new()
        {
            _responseHandlers[cmd] = new T();
        }

        public void AddSystemResponseHandler<T>(int cmd) where T : IServerResponeHandler, new()
        {
            _systemHandlers[cmd] = new T();
        }

        /// <summary>
        ///     Xóa sự kiện lắng nghe response từ server
        /// </summary>
        /// <param name="cmd">Command string</param>
        public void RemoveServerResponseHandler(int cmd)
        {
            _responseHandlers.Remove(cmd);
        }

        public void RemoveSystemResponseHandler(int cmd)
        {
            _systemHandlers.Remove(cmd);
        }

        /// <summary>
        ///     Xử lý server response
        /// </summary>
        /// <param name="user">KingUser</param>
        /// <param name="kMsg">KingMessage</param>
        /// <returns>True nếu xử lý thành công, false nếu không tìm thấy handler</returns>
        public bool HandleServerResponse(KingMessage kMsg)
        {
            try
            {
                int controllerId = kMsg.GetControllerId();
                int requestId = kMsg.GetRequestId();
                if (controllerId == StaticController.SYSTEM_CONTROLLER && _systemHandlers.TryGetValue(requestId, out IServerResponeHandler systemHandler))
                {
                    systemHandler.HandleServerRespone(kMsg);
                    return true;
                } 
                
                if (controllerId == StaticController.EXTENSION_CONTROLLER && _responseHandlers.TryGetValue(requestId, out IServerResponeHandler handler))
                {
                    handler.HandleServerRespone(kMsg);
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[HandlerManager|HandleClientRequest] " + (e.StackTrace ?? e.Message));
                return true;
            }

            return false;
        }

        #region Singleton Pattern

        private static readonly Lazy<HandlerManager> _instance = new Lazy<HandlerManager>(() => new HandlerManager());
        public static HandlerManager Instance => _instance.Value;

        #endregion
    }
}