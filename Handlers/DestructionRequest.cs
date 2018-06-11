using System;
using System.Numerics;
using Rocket.API.User;
using WreckingBall.Misc;

namespace WreckingBall.Handlers
{
    public class DestructionRequest
    {
        public IUser User { get; set; }
        public char [] Filters { get; set; }
        public uint Radius { get; set; }
        public Vector3 Position { get; set; }
        public WreckType WreckType { get; set; }
        public ulong SteamID { get; set; }
        public ushort ItemID { get; set; }

        public DateTime RequestAdded;

        public DestructionRequest (IUser user, string filter, uint radius, Vector3 position, WreckType wreckType, ulong steamID, ushort itemID)
        {
            User = user;
            Filters = filter.ToCharArray();
            Radius = radius;
            Position = position;
            WreckType = wreckType;
            SteamID = steamID;
            ItemID = itemID;
            RequestAdded = DateTime.Now;
        }
    }
}