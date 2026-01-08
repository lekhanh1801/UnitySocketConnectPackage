using System.Collections.Generic;
using System.Threading;

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
            // var playerId = msg.ReadUInt8();
            // var userId = msg.ReadUInt32();
            // var displayName = msg.ReadUtf8String();
            // var avatarUrl = msg.ReadUtf8String();
            // var countryCode = msg.ReadUInt8();
            // var booster = msg.ReadUInt8();
            // var trophy = msg.ReadUInt32();
            // var teamName = msg.ReadUtf8String();
            //
            // Debug.LogWarning("playerId "+playerId);
            // Debug.LogWarning("userId "+userId);
            // Debug.LogWarning("displayName "+displayName);
            // Debug.LogWarning("avatarUrl "+avatarUrl);
            // Debug.LogWarning("countryCode "+countryCode);
            // Debug.LogWarning("booster "+booster);
            // Debug.LogWarning("trophy "+trophy);
            // Debug.LogWarning("teamName "+teamName);

        }
    }
}