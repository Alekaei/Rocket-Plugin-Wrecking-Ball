using SDG.Unturned;
using Steamworks;
using System;
using UnityEngine;

namespace WreckingBall
{
    public static class Extensions
    {
        public static bool RegionOutOfRange(this Vector3 point, int x, int y, uint radius)
        {
            Vector3 regionPoint;
            Regions.tryGetPoint((byte)x, (byte)y, out regionPoint);
            // region center point.
            regionPoint += new Vector3(64, 0, 64);
            if (Vector3.Distance(regionPoint, new Vector3(point.x, 0f, point.z)) > radius + 92)
                return true;
            return false;
        }

        // Returns a ulong from a string on out, and returns true if it is a valid SteamID.
        public static bool isCSteamID(this string sCSteamID, out ulong ulCSteamID)
        {
            ulCSteamID = 0;
            if (ulong.TryParse(sCSteamID, out ulCSteamID))
            {
                if ((ulCSteamID >= 0x0110000100000000 && ulCSteamID <= 0x0170000000000000) || ulCSteamID == 0)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
