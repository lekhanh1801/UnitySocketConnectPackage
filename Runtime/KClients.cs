using System;
using System.Threading.Tasks;
using UnityEngine;

using WebSocketClientPackage.Runtime.Connections;
using WebSocketClientPackage.Runtime.enums;
using WebSocketClientPackage.Runtime.ios;
using WebSocketClientPackage.Runtime.managers;
using WebSocketClientPackage.Runtime.utils;
using WebSocketClientPackage.Runtime.models;
using WebSocketClientPackage.WebSocketClientPackage.Runtime.connections;

namespace WebSocketClientPackage.Runtime
{
    public class KClients
    {
        private static KClients _instance;
        public static KClients Instance => _instance ??= new KClients();

        private SocketClientConnection _socketClient;
        private WebSocketClientConnection _wsClient;
        private WebSocketClientConnection _wssClient;
        private ClientState _clientState = ClientState.NONE;
        private KClients()
        {
             HandlerManager.Instance.Init();
        }

        public async Task ConnectToServer(UserInfo dataLogin)
        {
            if (StaticConfig.SOCKET_ENABLE)
            {
                try
                {
                    _socketClient = new SocketClientConnection(StaticConfig.SERVER_IP, StaticConfig.SERVER_SOCKET_PORT);
                    _socketClient.OnConnected += SocketHandler.Instance.OnConnected;
                    _socketClient.OnDisconnected += SocketHandler.Instance.OnDisconnected;
                    _socketClient.OnError += SocketHandler.Instance.OnError;
                    _socketClient.OnReconnecting += SocketHandler.Instance.OnReConnect;
                    _socketClient.OnBinaryMessageReceived += SocketHandler.Instance.OnMessage;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    throw;
                }
                finally
                {
                    _clientState = ClientState.SOCKET_CLIENT;
                }
            }

            if (StaticConfig.WS_ENABLE)
            {
                try
                {
                    // string strData = JsonSerializer.Serialize(dataLogin);
                    string strData = JsonUtility.ToJson(dataLogin);
                    Debug.Log("strData " + strData);
                    string url = $"ws://{StaticConfig.SERVER_IP}:{StaticConfig.SERVER_WS_PORT}?prs=" + strData;
                    _wsClient = new WebSocketClientConnection(url);
                    _wsClient.OnConnected += WebSocketHandler.Instance.OnConnected;
                    _wsClient.OnDisconnected += WebSocketHandler.Instance.OnDisconnected;
                    _wsClient.OnError += WebSocketHandler.Instance.OnError;
                    _wsClient.OnReconnecting += WebSocketHandler.Instance.OnReConnect;
                    _wsClient.OnBinaryMessageReceived += WebSocketHandler.Instance.OnMessage;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    throw;
                }
                finally
                {
                    _clientState = ClientState.WS_CLIENT;
                }
            }

            if (StaticConfig.WSS_ENABLE)
            {
                try
                {
                    string url =
                        $"wss://{StaticConfig.SERVER_IP}:{StaticConfig.SERVER_WS_PORT}?prs={{\"uid\":\"33127\",\"cur\":\"usdt\",\"ipf\":true,\"cn\":\"869\",\"dn\":\"phahl004\",\"avt\":\"\",\"isBot\":false,\"isAdmin\":false,\"pid\":0,\"lang\":\"en\",\"utid\":0,\"vip\":1}}";
                    _wsClient = new WebSocketClientConnection(url);
                    _wsClient.OnConnected += WSSHandler.Instance.OnConnected;
                    _wsClient.OnDisconnected += WSSHandler.Instance.OnDisconnected;
                    _wsClient.OnError += WSSHandler.Instance.OnError;
                    _wsClient.OnReconnecting += WSSHandler.Instance.OnReConnect;
                    _wsClient.OnBinaryMessageReceived += WSSHandler.Instance.OnMessage;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    throw;
                }
                finally
                {
                    _clientState = ClientState.WSS_CLIENT;
                }
            }
        }

        public async Task SendMessageAsync(byte[] msg)
        {
            switch (_clientState)
            {
                case ClientState.SOCKET_CLIENT:
                    _socketClient.SendAsync(msg);
                    break;
                case ClientState.WS_CLIENT:
                    _wsClient.SendAsync(msg);
                    break;
                case ClientState.WSS_CLIENT:
                    _wssClient.SendAsync(msg);
                    break;
                default:
                    break;
            }
        }

        public async Task CloseAsync()
        {
            switch (_clientState)
            {
                case ClientState.SOCKET_CLIENT:
                    _socketClient.CloseAsync();
                    break;
                case ClientState.WS_CLIENT:
                    _wsClient.CloseAsync();
                    break;
                case ClientState.WSS_CLIENT:
                    _wssClient.CloseAsync();
                    break;
                default:
                    break;
            }
        }

        public IConnection ClientConnection()
        {
            switch (_clientState)
            {
                case ClientState.SOCKET_CLIENT:
                    return _socketClient;
                case ClientState.WS_CLIENT:
                    return _wsClient;
                case ClientState.WSS_CLIENT:
                    return _wssClient;
                default:
                    break;
            }
            return _wsClient;
        }

        public SocketClientConnection SocketClient
        {
            get => _socketClient;
            set => _socketClient = value;
        }

        public WebSocketClientConnection WsClient
        {
            get => _wsClient;
            set => _wsClient = value;
        }

        public WebSocketClientConnection WssClient
        {
            get => _wssClient;
            set => _wssClient = value;
        }
    }
}