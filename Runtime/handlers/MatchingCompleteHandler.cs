using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PlasticGui.WorkspaceWindow.Topbar;
using UnityEngine;
using WebSocketClientPackage.Runtime.handlers.base_handlers;
using WebSocketClientPackage.Runtime.managers;
using WebSocketClientPackage.Runtime.models;
using WebSocketClientPackage.Runtime.protocols;

namespace WebSocketClientPackage.Runtime.handlers
{
    public class MatchingCompleteHandler : IServerResponeHandler
    {
        public async void HandleServerRespone(KingMessage msg)
        {
            string roomName = "";
            MatchingManager.Instance.IsSuccessMatch = msg.ReadBool();
            int matchUserListLength = msg.ReadUInt8();
            List<UserInfo> matchUserList = new List<UserInfo>();
            for (int i = 0; i < matchUserListLength; i++)
            {
                UserInfo userInfo = new UserInfo()
                {
                    uid = msg.ReadUInt32().ToString(),
                    lv = msg.ReadUInt8(),
                    sex = msg.ReadUInt8(),
                    dn = msg.ReadUtf8String(),
                    avt = msg.ReadUtf8String(),
                    coin = msg.ReadUInt32(),
                    rn = msg.ReadUtf8String()
                };
                matchUserList.Add(userInfo);
                roomName = userInfo.rn;
            }

            if (MatchingManager.Instance.IsSuccessMatch)
            {
                await KClients.Instance.CloseAsync();
                string json = "{\"uid\":\"33127\",\"cur\":\"usdt\",\"ipf\":true,\"cn\":\"1\",\"dn\":\"phahl004\",\"avt\":\"\",\"isBot\":false,\"isAdmin\":false,\"pid\":0,\"lang\":\"en\",\"utid\":0,\"vip\":1}";
                UserInfo user = JsonUtility.FromJson<UserInfo>(json);
                user.rn = roomName;
                user.rm = 1;
                new Thread( async () =>
                {
                    Thread.Sleep(100);
                    await KClients.Instance.ConnectToServer(user);
                }).Start();
            }
        }
    }
}