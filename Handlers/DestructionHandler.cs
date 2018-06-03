using Rocket.API.Permissions;
using Rocket.API.Player;
using Rocket.API.Scheduler;
using Rocket.API.User;
using Rocket.Core.I18N;
using Rocket.UnityEngine.Extensions;
using Rocket.Unturned.Player;
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
		private IPermissionProvider permissionProvider;

		private DateTime lastDestructionRun;
		private int amountDestroyed;

		public DestructionHandler (WreckingBallPlugin plugin, ITaskScheduler taskScheduler, IPermissionProvider permissionProvider)
		{
			this.wreckPlugin = plugin;
			this.taskScheduler = taskScheduler;
			this.permissionProvider = permissionProvider;

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
			lastDestructionRun = DateTime.Now;
			taskScheduler.ScheduleEveryFrame (wreckPlugin, DestructionRun);

			if (wreckPlugin.ConfigurationInstance.EnableVehicleCap)
			{
				lastVehicleCapRun = DateTime.Now;
				taskScheduler.ScheduleEveryFrame (wreckPlugin, VehicleCapRun);
			}
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
					{
						if (data.owner != request.steamID)
							continue;
						if (request.wreckType != WreckType.Scan && permissionProvider.CheckHasAnyPermission (wreckPlugin.Container.Resolve<IPlayerManager> ().GetPlayer (request.steamID.ToString ()), new string []
							{
								"wreckingball.skip.barricade",
								"wreckingball.skip.*",
								"wreckingball.*"
							}) == PermissionResult.Grant)
							continue;
					}

					if (request.itemID != 0)
						if (data.barricade.id != request.itemID)
							continue;

					if (request.filters != null)
						if (!wreckPlugin.ConfigurationInstance.Elements.Any (c => request.filters.Contains (c.CategoryId) && c.Id == data.barricade.id || request.filters.Contains ('*')))
							continue;

					if (request.wreckType != WreckType.Scan && permissionProvider.CheckHasAnyPermission (request.user, new string []
						{
							"wreckingball.skip.barricade",
							"wreckingball.skip.*",
							"wreckingball.*"
						}) == PermissionResult.Grant)
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
					{
						if (data.owner != request.steamID)
							continue;
							if (request.wreckType != WreckType.Scan && permissionProvider.CheckHasAnyPermission (wreckPlugin.Container.Resolve <IPlayerManager> ().GetPlayer (request.steamID.ToString ()), new string []
							{
								"wreckingball.skip.structure",
								"wreckingball.skip.*",
								"wreckingball.*"
							}) == PermissionResult.Grant)
							continue;
					}

					if (request.itemID != 0)
						if (data.structure.id != request.itemID)
							continue;

					if (request.filters != null)
						if (!wreckPlugin.ConfigurationInstance.Elements.Any (c => request.filters.Contains (c.CategoryId) && c.Id == data.structure.id || request.filters.Contains ('*')))
							continue;

					if (request.wreckType != WreckType.Scan && permissionProvider.CheckHasAnyPermission (request.user, new string []
						{
							"wreckingball.skip.structure",
							"wreckingball.skip.*",
							"wreckingball.*"
						}) == PermissionResult.Grant)
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
				{
					if (vehicle.lockedOwner.m_SteamID != request.steamID)
						continue;
					if (request.wreckType != WreckType.Scan && permissionProvider.CheckHasAnyPermission (wreckPlugin.Container.Resolve<IPlayerManager> ().GetPlayer (request.steamID.ToString ()), new string []
						{
							"wreckingball.skip.vehicle",
							"wreckingball.skip.*",
							"wreckingball.*"
						}) == PermissionResult.Grant)
						continue;
				}

				if (request.itemID != 0)
					if (vehicle.id != request.itemID)
						continue;

				if (request.filters != null)
					if (!wreckPlugin.ConfigurationInstance.Elements.Any (c => request.filters.Contains (c.CategoryId) && c.Id == vehicle.id || request.filters.Contains ('*')))
						continue;

				if (request.wreckType != WreckType.Scan && permissionProvider.CheckHasAnyPermission (request.user, new string []
					{
						"wreckingball.skip.vehicle",
						"wreckingball.skip.*",
							"wreckingball.*"
					}) == PermissionResult.Grant)
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
			if ((DateTime.Now - lastDestructionRun).TotalMilliseconds <= (1000/wreckPlugin.ConfigurationInstance.DestructionInterval) &&
				amountDestroyed >= wreckPlugin.ConfigurationInstance.DestructionsPerInterval)
				return;

			KeyValuePair<int, object> nextDestroy = destroyQueue.First ();

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

		private DateTime lastVehicleCapRun;
		public void VehicleCapRun ()
		{
			if ((DateTime.Now - lastVehicleCapRun).TotalSeconds < wreckPlugin.ConfigurationInstance.VehicleDestructionInterval)
				return;
			int runCount = 0;

			while (VehicleManager.vehicles.Count > wreckPlugin.ConfigurationInstance.MaxVehiclesAllowed)
			{
				InteractableVehicle vehicle = VehicleManager.vehicles.FirstOrDefault ();

				if (vehicle == null)
				{
					lastVehicleCapRun = DateTime.Now;
					return;
				}

				if (vehicle.isLocked && permissionProvider.CheckHasAnyPermission (wreckPlugin.Container.Resolve <IPlayerManager> ().GetPlayer (vehicle.lockedOwner.m_SteamID.ToString ()), new string []
					{
						"wreck.skip.vehicle",
						"wreck.skip.*",
						"wreck.*"
					}) == PermissionResult.Grant)
				{
					lastVehicleCapRun = DateTime.Now;
					return;
				}

				bool skip = false;
				int count = 0;

				BarricadeManager.tryGetPlant (vehicle.transform, out byte x, out byte y, out ushort plant, out BarricadeRegion region);
				count += region.barricades.Count;

				if (wreckPlugin.ConfigurationInstance.KeepVehiclesWithSigns && region.drops.Any (c => c.interactable is InteractableSign sign && sign.text.Contains (wreckPlugin.ConfigurationInstance.VehicleSignFlag)))
					continue;

				foreach (var tranCar in vehicle.trainCars)
				{
					BarricadeManager.tryGetPlant (vehicle.transform, out x, out y, out plant, out BarricadeRegion tRegion);
					count += tRegion.barricades.Count;
					if (wreckPlugin.ConfigurationInstance.KeepVehiclesWithSigns && tRegion.drops.Any (c => c.interactable is InteractableSign sign && sign.text.Contains (wreckPlugin.ConfigurationInstance.VehicleSignFlag)))
					{
						skip = true;
						break;
					}
				}

				if (wreckPlugin.ConfigurationInstance.LowElementCountOnly)
					if (count >= wreckPlugin.ConfigurationInstance.MinElementCount)
						skip = true;

				if (!skip)
					vehicle.askDamage (ushort.MaxValue, true);

				runCount++;
				if (runCount > 15)
					break;
			}

			lastVehicleCapRun = DateTime.Now;
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
