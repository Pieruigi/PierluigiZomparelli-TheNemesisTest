using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheNemesis
{
    public class PlayerCustomPropertyUtility
    {
        public static readonly string TeamKey = "tm";

        public static byte GetTeamId(Player player)
        {
            object ret = 0;
            if (!player.CustomProperties.TryGetValue(TeamKey, out ret))
                ret = (byte)0;
            return (byte)ret;
        }

        public static void SetTeamId(Player player, byte value)
        {
            if (player.CustomProperties.ContainsKey(TeamKey))
                player.CustomProperties[TeamKey] = value;
            else
                player.CustomProperties.Add(TeamKey, value);
        }

        
    }

}
