using Rocket.API.User;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace WreckingBall
{
	public class DestructionHandler
	{
		private WreckingBallPlugin wreckPlugin;
		private List<DestructionObject> pendingConfirmation;
		private List<DestructionObject> pendingDestruction;

		public void AddRequest (IUser user, string filter, uint radius, Vector3 position, WreckType wreckType, ulong steamID, ushort itemID)
		{
			if (pendingConfirmation.Any (c => c.user == user))
				pendingConfirmation.Remove (pendingConfirmation.FirstOrDefault (c => c.user == user));
			pendingConfirmation.Add (new DestructionObject (user, filter, radius, position, wreckType, steamID, itemID));
		}

		public bool ConfirmRequest (IUser user)
		{
			if (pendingConfirmation.Any (c => c.user == user))
			{
				DestructionObject destructionObject = pendingConfirmation.FirstOrDefault (c => c.user == user);
				pendingDestruction.Add (destructionObject);
				pendingConfirmation.Remove (destructionObject);
				return true;
			}
			return false;
		}

		public bool AbortRequest (IUser user)
		{
			if (pendingConfirmation.Any (c => c.user == user))
			{
				pendingConfirmation.Remove (pendingConfirmation.FirstOrDefault (c => c.user == user));
				return true;
			}
			return false;
		}

		public void RunEveryFrame ()
		{
			if (pendingDestruction.Count == 0)
				return;
			DestructionObject destructionObject = pendingDestruction.First ();
			if ((DateTime.Now - DestructionProcessing.lastRunTimeWreck).TotalSeconds > (1 / wreckPlugin.ConfigurationInstance.DestructionRate))
			{
				DestructionProcessing.lastRunTimeWreck = DateTime.Now;
				if (DestructionProcessing.processing)
					DestructionProcessing.DestructionLoop (WreckType.Wreck);
				if (DestructionProcessing.cleanupProcessingBuildables)
					DestructionProcessing.DestructionLoop (WreckType.Cleanup);
			}
			if (ConfigurationInstance.EnableCleanup)
				DestructionProcessing.HandleCleanup ();
			if (ConfigurationInstance.EnableVehicleCap)
				DestructionProcessing.HandleVehicleCap ();
		}
	}

	public class DestructionObject
	{
		public IUser user;
		public string filter;
		public uint radius;
		public Vector3 position;
		public WreckType wreckType;
		public ulong steamID;
		public ushort itemID;

		public DestructionObject (IUser user, string filter, uint radius, Vector3 position, WreckType wreckType, ulong steamID, ushort itemID)
		{
			this.user = user;
			this.filter = filter;
			this.radius = radius;
			this.position = position;
			this.wreckType = wreckType;
			this.steamID = steamID;
			this.itemID = itemID;
		}
	}
}
