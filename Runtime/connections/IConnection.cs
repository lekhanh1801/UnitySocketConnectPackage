using System;
using System.Collections.Generic;

namespace WebSocketClientPackage.WebSocketClientPackage.Runtime.connections
{
    public interface IConnection
    {
        #region Events

        public event Action<string> OnMessageReceived;

        public event Action<byte[]> OnBinaryMessageReceived;

        public event Action OnConnected;

        public event Action OnDisconnected;

        public event Action<string> OnError;

        public event Action<int> OnReconnecting;

        #endregion
    }
}