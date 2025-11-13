namespace WebSocketClientPackage.Runtime.ios
{
    public class WSSHandler
    {
        private static WSSHandler _instance;
        public static WSSHandler Instance => _instance ??= new WSSHandler();

        /// <summary>
        /// Khởi tạo SocketHandler
        /// </summary>
        private WSSHandler()
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