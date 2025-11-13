namespace WebSocketClientPackage.Runtime.managers
{
    public class MatchingManager
    {
        private static MatchingManager _instance;
        public static MatchingManager Instance => _instance ??= new MatchingManager();

        /// <summary>
        /// Khởi tạo SocketHandler
        /// </summary>
        private MatchingManager()
        {
        }

        private bool _isSuccessMatch = false;

        public bool IsSuccessMatch
        {
            get => _isSuccessMatch;
            set => _isSuccessMatch = value;
        }
    }
}