
using System;
using System.Collections.Generic;
using System.IO;
using WebSocketClientPackage.Runtime.protocols;
using UnityEngine;
using WebSocketClientPackage.Runtime;

namespace WebSocketClientPackage.Runtime.helpers
{
public static class KingHelper
{
    /** <controllerId, Set<requestId>> */
    private static Dictionary<int, HashSet<int>> _logControllerRequestIds = new Dictionary<int, HashSet<int>>();

    /// <summary>
    /// Set log controller request ids.
    /// </summary>
    /// <param name="logControllerRequestIds">The log controller request ids.</param>
    public static void SetLogControllerRequestIds(Dictionary<int, HashSet<int>> logControllerRequestIds)
    {
        _logControllerRequestIds = logControllerRequestIds ?? new Dictionary<int, HashSet<int>>();
    }

    /// <summary>
    /// Sends a message to a user.
    /// </summary>
    /// <param name="user">The user to send the message to.</param>
    /// <param name="kMsg">The KingMessage instance to send.</param>
    public static void Send(KingMessage kMsg)
    {
        KClients.Instance.SendMessageAsync(kMsg.GetFinalBuffer());

        int controllerId = kMsg.GetControllerId();
        int requestId = kMsg.GetRequestId();

        if (_logControllerRequestIds.ContainsKey(controllerId))
        {
            HashSet<int> logRequestIds = _logControllerRequestIds[controllerId];
            if (logRequestIds.Contains(requestId))
            {
                Debug.Log($"[KingHelper|Send] Send [{controllerId}|{requestId}]");
            }
        }
    }
}
}



