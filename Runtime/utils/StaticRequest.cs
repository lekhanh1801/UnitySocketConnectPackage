namespace WebSocketClientPackage.Runtime.utils
{
    public static class StaticRequest
    {
        public static int PING = -2;
        public static int LOGIN_DEV = -1;
        public static int LOGIN = 1;
        public static int LOGIN_RESPONSE = 2;
        public static int USER_PING = 3;
        public static int USER_LOST_PING = -3;
        public static int USER_PING_DETECT_NETWORK = 4;
        public static int PONG = 6;

    
        public static int LOGIN_OTHER_SESSION = 23;
        public static int KICK_USER_AFK_LOBBY = 5;
    
        public static int JOIN_ROOM = 7;
        public static int LEAVE_ROOM = 10;
        public static int VIEWER_TO_PLAYER = 17;
        public static int PLAYER_TO_VIEWER = 18;
    
    
        public static int ON_USER_ENTER_ROOM = 8;
        public static int ON_USER_LEAVE_ROOM = 11;
        public static int ON_OTHER_USER_LEAVE_ROOM = 12;
        public static int MASTER_MATCHING = 19;
    }
}