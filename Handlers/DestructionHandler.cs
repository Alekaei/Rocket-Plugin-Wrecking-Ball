using Rocket.API.Scheduler;
using Rocket.API.User;
using Rocket.Core.I18N;
using Rocket.UnityEngine.Extensions;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace WreckingBall
{
	public class DestructionHandler
	{
		private WreckingBallPlugin wreckPlugin;
		private List<DestructionRequest> pendingConfirmation;
		private List<DestructionRequest> scanQueue;
		private Dictionary<int, object> destroyQueue; 

		private ITaskScheduler taskScheduler;

		private DateTime lastDestructionRun;
		private int amountDestroyed;

		public DestructionHandler (WreckingBallPlugin plugin, ITaskScheduler taskScheduler)
		{
			this.wreckPlugin = plugin;
			this.taskScheduler = taskScheduler;

			pendingConfirmation = new List<DestructionRequest> ();
			scanQueue = new List<DestructionRequest> ();
			destroyQueue = new Dictionary<int, object> ();
		}

		public void Load ()
		{
			pendingConfirmation = new List<DestructionRequest> ();
			scanQueue = new List<DestructionRequest> ();
			destroyQueue = new Dictionary<int, object> ();

			taskScheduler.ScheduleEveryFrame (wreckPlugin, ScanRun);
			taskScheduler.ScheduleEveryFrame (wreckPlugin, DestructionRun);
		}

		public void AddRequest (IUser user, string filter, uint radius, Vector3 position, WreckType wreckType, ulong steamID, ushort itemID)
		{
			if (pendingConfirmation.Any (c => c.user == user))
			{
				pendingConfirmation.Remove (pendingConfirmation.FirstOrDefault (c => c.user == user));
				return;
			}
			pendingConfirmation.Add (new DestructionRequest (user, filter, radius, position, wreckType, steamID, itemID));
		}

		public void ConfirmRequest (IUser user)
		{
			if (pendingConfirmation.Any (c => c.user == user))
			{
				DestructionRequest destructionRequest = pendingConfirmation.FirstOrDefault (c => c.user == user);
				if ((DateTime.Now - destructionRequest.requestAdded).TotalSeconds >= 120)
				{
					pendingConfirmation.Remove (destructionRequest);
					user.SendLocalizedMessage (wreckPlugin.Translations, "wreckingball_no_request");
					return;
				}
				scanQueue.Add (destructionRequest);
				pendingConfirmation.Remove (destructionRequest);
				user.SendLocalizedMessage (wreckPlugin.Translations, "wreckingball_confirmed");
				return;
			}
			user.SendLocalizedMessage (wreckPlugin.Translations, "wreckingball_no_request");
		}

		public void AbortRequest (IUser user)
		{
			if (pendingConfirmation.Any (c => c.user == user))
			{
				var destructionRequest = pendingConfirmation.FirstOrDefault (c => c.user == user);
				if ((DateTime.Now - destructionRequest.requestAdded).TotalSeconds >= 120)
				{
					pendingConfirmation.Remove (destructionRequest);
					user.SendLocalizedMessage (wreckPlugin.Translations, "wreckingball_no_request");
					return;
				}
				pendingConfirmation.Remove (destructionRequest);
				user.SendLocalizedMessage (wreckPlugin.Translations, "wreckingball_aborted");
				return;
			}
			user.SendLocalizedMessage (wreckPlugin.Translations, "wreckingball_no_request");
		}

		public void ScanRun ()
		{
			if (scanQueue.Count == 0)
				return;
			DestructionRequest request = scanQueue.First ();

			int objectsFound = 0;
			
			foreach (BarricadeRegion region in BarricadeManager.regions)
			{
				foreach (BarricadeData data in region.barricades)
				{
					if (Vector3.Distance (request.position, data.point.ToSystemVector ()) > request.radius)
						continue;

					if (destroyQueue.ContainsKey (data.GetHashCode ()))
						continue;

					if (request.steamID != 0)
						if (data.owner != request.steamID)
							continue;

					if (request.itemID != 0)
						if (data.barricade.id != request.itemID)
							continue;

					if (request.filters != null)
						if (!wreckPlugin.ConfigurationInstance.Elements.Any (c => request.filters.Contains (c.CategoryId) && c.Id == data.barricade.id || request.filters.Contains ('*')))
							continue;

					objectsFound++;
					if (request.wreckType == WreckType.Scan)
						continue;
					destroyQueue.Add (data.GetHashCode (), data);
				}
			}

			foreach (StructureRegion region in StructureManager.regions)
			{
				foreach (StructureData data in region.structures)
				{
					if (Vector3.Distance (request.position, data.point.ToSystemVector ()) > request.radius)
						continue;

					if (destroyQueue.ContainsKey (data.GetHashCode ()))
						continue;

					if (request.steamID != 0)
						if (data.owner != request.steamID)
							continue;

					if (request.itemID != 0)
						if (data.structure.id != request.itemID)
							continue;

					if (request.filters != null)
						if (!wreckPlugin.ConfigurationInstance.Elements.Any (c => request.filters.Contains (c.CategoryId) && c.Id == data.structure.id || request.filters.Contains ('*')))
							continue;

					objectsFound++;
					if (request.wreckType == WreckType.Scan)
						continue;
					destroyQueue.Add (data.GetHashCode (), data);
				}
			}

			foreach (InteractableVehicle vehicle in VehicleManager.vehicles)
			{
				if (Vector3.Distance (request.position, vehicle.transform.position.ToSystemVector ()) > request.radius)
					continue;

				if (destroyQueue.ContainsKey (vehicle.GetHashCode ()))
					continue;

				if (request.steamID != 0)
					if (vehicle.lockedOwner.m_SteamID != request.steamID)
						continue;

				if (request.itemID != 0)
					if (vehicle.id != request.itemID)
						continue;

				if (request.filters != null)
					if (!wreckPlugin.ConfigurationInstance.Elements.Any (c => request.filters.Contains (c.CategoryId) && c.Id == vehicle.id || request.filters.Contains ('*')))
						continue;

				objectsFound++;
				if (request.wreckType == WreckType.Scan)
					continue;
				destroyQueue.Add (vehicle.GetHashCode (), vehicle);
			}

			if (request.filters.Contains ('A'))
			{
				foreach (Animal animal in AnimalManager.animals)
				{
					if (Vector3.Distance (request.position, animal.transform.position.ToSystemVector ()) > request.radius)
						continue;

					if (destroyQueue.ContainsKey (animal.GetHashCode ()))
						continue;

					if (request.itemID != 0)
						if (animal.id != request.itemID)
							continue;

					objectsFound++;
					if (request.wreckType == WreckType.Scan)
						continue;
					destroyQueue.Add (animal.GetHashCode (), animal);
				}
			}
			if (request.filters.Contains ('Z'))
			{
				foreach (ZombieRegion region in ZombieManager.regions)
				{
					foreach (Zombie zombie in region.zombies)
					{
						if (Vector3.Distance (request.position, zombie.transform.position.ToSystemVector ()) > request.radius)
							continue;

						if (destroyQueue.ContainsKey (zombie.GetHashCode ()))
							continue;

						if (request.itemID != 0)
							if (zombie.id != request.itemID)
								continue;

						objectsFound++;
						if (request.wreckType == WreckType.Scan)
							continue;
						destroyQueue.Add (zombie.GetHashCode (), zombie);
					}
				}
			}

			request.user.SendLocalizedMessage (wreckPlugin.Translations, "wreckingball_scan", objectsFound);
			scanQueue.Remove (request);
			if (request.wreckType == WreckType.Scan)
				return;
			request.user.SendLocalizedMessage (wreckPlugin.Translations, "wreckingball_added_destruction", FormattedTimeUntilDestroyed ());
		}

		public void DestructionRun ()
		{
			if (destroyQueue.Count == 0)
				return;
			if ((DateTime.Now - lastDestructionRun).TotalMilliseconds <= wreckPlugin.ConfigurationInstance.DestructionRate && 
				amountDestroyed >= wreckPlugin.ConfigurationInstance.DestructionsPerInterval)
				return;

			KeyValuePair <int, object> nextDestroy = destroyQueue.First ();

			if (nextDestroy.Value is BarricadeData bData)
			{
				UnityEngine.Transform transform = UnityEngine.Physics.OverlapSphere (bData.point, 0.5f, 1 << LayerMasks.BARRICADE).Where (c => c.transform.position == bData.point).FirstOrDefault ()?.transform;
				if (transform == null)
					return;
				BarricadeManager.damage (transform, ushort.MaxValue, 1, false);
			}
			if (nextDestroy.Value is StructureData sData)
			{
				UnityEngine.Transform transform = UnityEngine.Physics.OverlapSphere (sData.point, 0.5f, 1 << LayerMasks.BARRICADE).Where (c => c.transform.position == sData.point).FirstOrDefault ()?.transform;
				if (transform == null)
					return;
				StructureManager.damage (transform, transform.position, ushort.MaxValue, 1, false);
			}
			if (nextDestroy.Value is InteractableVehicle vData)
			{
				vData.askDamage (ushort.MaxValue, true);
			}
			if (nextDestroy.Value is Animal aData)
			{
				aData.askDamage (byte.MaxValue, aData.transform.up, out EPlayerKill playerKill, out uint xp);
			}
			if (nextDestroy.Value is Zombie zData)
			{
				zData.askDamage (byte.MaxValue, zData.transform.up, out EPlayerKill playerKill, out uint xp);
			}

			destroyQueue.Remove (nextDestroy.Key);
			amountDestroyed++;
			if (amountDestroyed == wreckPlugin.ConfigurationInstance.DestructionsPerInterval)
				lastDestructionRun = DateTime.Now;
		}

		public string FormattedTimeUntilDestroyed ()
		{
			float time = (float) wreckPlugin.ConfigurationInstance.DestructionsPerInterval * (float) (destroyQueue.Count / wreckPlugin.ConfigurationInstance.DestructionsPerInterval);

			TimeSpan estimated = TimeSpan.FromSeconds (time);
			
			string formatted = "";

			if (estimated.Minutes > 0)
			{
				formatted += estimated.Minutes + ((estimated.Minutes > 1) ? " minutes" : " minute");
				if (estimated.Seconds > 0)
					formatted += " " + estimated.Seconds + ((estimated.Seconds > 1) ? " seconds" : " second");
			}
			else
				formatted += estimated.Seconds + ((estimated.Seconds == 1) ? " second" : " seconds");

			return formatted;
		}
	}

	public class DestructionRequest
	{
		public IUser user;
		public char [] filters;
		public uint radius;
		public Vector3 position;
		public WreckType wreckType;
		public ulong steamID;
		public ushort itemID;

		public DateTime requestAdded;

		public DestructionRequest (IUser user, string filter, uint radius, Vector3 position, WreckType wreckType, ulong steamID, ushort itemID)
		{
			this.user = user;
			this.filters = filter.Split ().Cast<char> ().ToArray ();
			this.radius = radius;
			this.position = position;
			this.wreckType = wreckType;
			this.steamID = steamID;
			this.itemID = itemID;
			this.requestAdded = DateTime.Now;
		}
	}
}
