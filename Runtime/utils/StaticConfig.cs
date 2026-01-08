using System;
using System.IO;
using UnityEngine;

namespace WebSocketClientPackage.Runtime.utils
{
    public static class StaticConfig
    {
        public static string GAME_NAME = "";
        public static string SERVER_IP = "192.168.1.52";
        public static string SERVER_DOMAIN = "";

        public static int SERVER_SOCKET_PORT = 0;
        public static int SERVER_WS_PORT = 62002;
        public static int SERVER_WSS_PORT = 0;

        public static string SSL_CONFIG = "";
        public static int SSL_RELOAD_TIME = 30000;

        public static bool SOCKET_ENABLE = false;
        public static bool WS_ENABLE = true;
        public static bool WSS_ENABLE = false;

        public static bool IS_ENABLE_DETECT_SOCKET_IDLE = false;
        public static int IDLE_MAX_MILI_SOCKET = 0;

        public static bool SKIP_UTF8_VALIDATION = false;
        public static bool IS_LOGIN_BY_QUERY_PARAM_TYPE = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            LoadFromJson();
        }

        public static void LoadFromJson()
        {
            try
            {
                TextAsset textFile = Resources.Load<TextAsset>("configuration/configuration");

                string jsoncontent = textFile.text;
                var configData = JsonUtility.FromJson<ConfigData>(jsoncontent);

                // Gán giá trị từ JSON vào các static properties
                GAME_NAME = configData.GAME_NAME ?? GAME_NAME;
                SERVER_IP = configData.SERVER_IP ?? SERVER_IP;
                SERVER_DOMAIN = configData.SERVER_DOMAIN ?? SERVER_DOMAIN;

                SERVER_SOCKET_PORT = configData.SERVER_SOCKET_PORT;
                SERVER_WS_PORT = configData.SERVER_WS_PORT;
                SERVER_WSS_PORT = configData.SERVER_WSS_PORT;

                SSL_CONFIG = configData.SSL_CONFIG ?? SSL_CONFIG;
                SSL_RELOAD_TIME = configData.SSL_RELOAD_TIME;

                SOCKET_ENABLE = configData.SOCKET_ENABLE;
                WS_ENABLE = configData.WS_ENABLE;
                WSS_ENABLE = configData.WSS_ENABLE;
                Debug.Log("StaticConfig loaded successfully from JSON");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading config from JSON: {ex.Message}");
            }
        }

        // Class phụ để deserialize JSON
        [Serializable]
        private class ConfigData
        {
            public string GAME_NAME;
            public string SERVER_IP;
            public string SERVER_DOMAIN;
            public int SERVER_SOCKET_PORT;
            public int SERVER_WS_PORT;
            public int SERVER_WSS_PORT;
            public string SSL_CONFIG;
            public int SSL_RELOAD_TIME;
            public bool SOCKET_ENABLE;
            public bool WS_ENABLE;
            public bool WSS_ENABLE;
        }
    }
}