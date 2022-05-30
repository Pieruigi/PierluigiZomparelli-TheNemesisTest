using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheNemesis
{
    public class RoomCustomPropertyUtility
    {
        public static readonly string StartTimeKey = "st";
        public static readonly string MatchStateKey = "ms";
        public static readonly string BlueScoreKey = "bs";
        public static readonly string RedScoreKey = "rs";

        public static double GetStartTime(Room room)
        {
            object ret;
            if (!room.CustomProperties.TryGetValue(StartTimeKey, out ret))
                ret = (double)0;

            return (double)ret;
        }

        public static void SetStartTime(Room room, double value)
        {
            if (room.CustomProperties.ContainsKey(StartTimeKey))
                room.CustomProperties[StartTimeKey] = value;
            else
                room.CustomProperties.Add(StartTimeKey, value);
        }

        public static byte GetMatchState(Room room)
        {
            object ret;
            if (!room.CustomProperties.TryGetValue(MatchStateKey, out ret))
                ret = (byte)0;

            return (byte)ret;
        }

        public static void SetMatchState(Room room, byte value)
        {
            if (room.CustomProperties.ContainsKey(MatchStateKey))
                room.CustomProperties[MatchStateKey] = value;
            else
                room.CustomProperties.Add(MatchStateKey, value);
        }

        public static byte GetBlueScore(Room room)
        {
            object ret;
            if (!room.CustomProperties.TryGetValue(BlueScoreKey, out ret))
                ret = (byte)0;

            return (byte)ret;
        }

        public static void SetBlueScore(Room room, byte value)
        {
            if (room.CustomProperties.ContainsKey(BlueScoreKey))
                room.CustomProperties[BlueScoreKey] = value;
            else
                room.CustomProperties.Add(BlueScoreKey, value);
        }

        public static byte GetRedScore(Room room)
        {
            object ret;
            if (!room.CustomProperties.TryGetValue(RedScoreKey, out ret))
                ret = (byte)0;

            return (byte)ret;
        }

        public static void SetRedScore(Room room, byte value)
        {
            if (room.CustomProperties.ContainsKey(RedScoreKey))
                room.CustomProperties[RedScoreKey] = value;
            else
                room.CustomProperties.Add(RedScoreKey, value);
        }
    }

}
