using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.Permissions;
using Rocket.API.Player;
using Rocket.API.Scheduler;
using Rocket.API.User;
using Rocket.Core.I18N;
using Rocket.Core.Scheduler;
using Rocket.UnityEngine.Extensions;
using SDG.Unturned;
using UnityEngine;
using WreckingBall.Misc;
using Vector3 = System.Numerics.Vector3;

namespace WreckingBall.Handlers
{
	public class DestructionHandler
	{
		private readonly WreckingBallPlugin _wreckPlugin;
		private List<DestructionRequest> _pendingConfirmation;
		private List<DestructionRequest> _scanQueue;
		private Dictionary<int, object> _destroyQueue;

		private readonly ITaskScheduler _taskScheduler;
		private readonly IPermissionProvider _permissionProvider;

		private DateTime _lastDestructionRun;
		private int _amountDestroyed;

		public DestructionHandler (WreckingBallPlugin plugin, ITaskScheduler taskScheduler, IPermissionProvider permissionProvider)
		{
			_wreckPlugin = plugin;
			_taskScheduler = taskScheduler;
			_permissionProvider = permissionProvider;

			_pendingConfirmation = new List<DestructionRequest> ();
			_scanQueue = new List<DestructionRequest> ();
			_destroyQueue = new Dictionary<int, object> ();
		}

		public void Load ()
		{
			_pendingConfirmation = new List<DestructionRequest> ();
			_scanQueue = new List<DestructionRequest> ();
			_destroyQueue = new Dictionary<int, object> ();

			_taskScheduler.ScheduleEveryFrame (_wreckPlugin, ScanRun, "WreckingBallScan");
			_lastDestructionRun = DateTime.Now;
			_taskScheduler.ScheduleEveryFrame (_wreckPlugin, DestructionRun, "WreckingBallDestruction");

			if (_wreckPlugin.ConfigurationInstance.EnableVehicleCap)
			{
				_lastVehicleCapRun = DateTime.Now;
				_taskScheduler.ScheduleEveryFrame (_wreckPlugin, VehicleCapRun, "WreckingBallVehicleCap");
			}
		}

		public void AddRequest (IUser user, string filter, uint radius, Vector3 position, WreckType wreckType, ulong steamID, ushort itemID)
		{
			if (_pendingConfirmation.Any (c => Equals(c.User, user)))
			{
				_pendingConfirmation.Remove (_pendingConfirmation.FirstOrDefault (c => Equals(c.User, user)));
				return;
			}
			_pendingConfirmation.Add (new DestructionRequest (user, filter, radius, position, wreckType, steamID, itemID));
		}

		public void ConfirmRequest (IUser user)
		{
		    if (_pendingConfirmation.All(c => !Equals(c.User, user)))
		    {
		        user.SendLocalizedMessage(_wreckPlugin.Translations, "wreckingball_no_request");
		        return;
		    }

		    DestructionRequest destructionRequest = _pendingConfirmation.FirstOrDefault(c => Equals(c.User, user));
		    if (destructionRequest != null && (DateTime.Now - destructionRequest.RequestAdded).TotalSeconds >= 120)
		    {
		        _pendingConfirmation.Remove(destructionRequest);
		        user.SendLocalizedMessage(_wreckPlugin.Translations, "wreckingball_no_request");
		        return;
		    }

		    _scanQueue.Add(destructionRequest);
		    _pendingConfirmation.Remove(destructionRequest);
		    user.SendLocalizedMessage(_wreckPlugin.Translations, "wreckingball_confirmed");
		}

		public void AbortRequest (IUser user)
		{
		    if (_pendingConfirmation.All(c => !Equals(c.User, user)))
		    {
		        user.SendLocalizedMessage(_wreckPlugin.Translations, "wreckingball_no_request");
		        return;
		    }

		    var destructionRequest = _pendingConfirmation.FirstOrDefault(c => Equals(c.User, user));
		    if (destructionRequest != null && (DateTime.Now - destructionRequest.RequestAdded).TotalSeconds >= 120)
		    {
		        _pendingConfirmation.Remove(destructionRequest);
		        user.SendLocalizedMessage(_wreckPlugin.Translations, "wreckingball_no_request");
		        return;
		    }

		    _pendingConfirmation.Remove(destructionRequest);
		    user.SendLocalizedMessage(_wreckPlugin.Translations, "wreckingball_aborted");
		}

		public void ScanRun ()
		{
			if (_scanQueue.Count == 0)
				return;
			DestructionRequest request = _scanQueue.First ();

			int objectsFound = 0;

			foreach (BarricadeRegion region in BarricadeManager.regions)
			{
				foreach (BarricadeData data in region.barricades)
				{
					if (Vector3.Distance (request.Position, data.point.ToSystemVector ()) > request.Radius)
						continue;

					if (_destroyQueue.ContainsKey (data.GetHashCode ()))
						continue;

					if (request.SteamID != 0)
					{
						if (data.owner != request.SteamID)
							continue;
						if (request.WreckType != WreckType.Scan && _permissionProvider.CheckHasAnyPermission (_wreckPlugin.Container.Resolve<IPlayerManager> ().GetPlayer (request.SteamID.ToString ()), "wreckingball.skip.barricade", "wreckingball.skip.*", "wreckingball.*") == PermissionResult.Grant)
							continue;
					}

					if (request.ItemID != 0)
						if (data.barricade.id != request.ItemID)
							continue;

					if (request.Filters != null)
						if (!_wreckPlugin.ConfigurationInstance.Elements.Any (c => request.Filters.Contains (c.CategoryId) && c.Id == data.barricade.id || request.Filters.Contains ('*')))
							continue;

					if (request.WreckType != WreckType.Scan && _permissionProvider.CheckHasAnyPermission (request.User, "wreckingball.skip.barricade", "wreckingball.skip.*", "wreckingball.*") == PermissionResult.Grant)
						continue;

					objectsFound++;
					if (request.WreckType == WreckType.Scan)
						continue;
					_destroyQueue.Add (data.GetHashCode (), data);
				}
			}

			foreach (StructureRegion region in StructureManager.regions)
			{
				foreach (StructureData data in region.structures)
				{
					if (Vector3.Distance (request.Position, data.point.ToSystemVector ()) > request.Radius)
						continue;

					if (_destroyQueue.ContainsKey (data.GetHashCode ()))
						continue;

					if (request.SteamID != 0)
					{
						if (data.owner != request.SteamID)
							continue;
							if (request.WreckType != WreckType.Scan && _permissionProvider.CheckHasAnyPermission (_wreckPlugin.Container.Resolve <IPlayerManager> ().GetPlayer (request.SteamID.ToString ()), "wreckingball.skip.structure", "wreckingball.skip.*", "wreckingball.*") == PermissionResult.Grant)
							continue;
					}

					if (request.ItemID != 0)
						if (data.structure.id != request.ItemID)
							continue;

					if (request.Filters != null)
						if (!_wreckPlugin.ConfigurationInstance.Elements.Any (c => request.Filters.Contains (c.CategoryId) && c.Id == data.structure.id || request.Filters.Contains ('*')))
							continue;

					if (request.WreckType != WreckType.Scan && _permissionProvider.CheckHasAnyPermission (request.User, "wreckingball.skip.structure", "wreckingball.skip.*", "wreckingball.*") == PermissionResult.Grant)
						continue;

					objectsFound++;
					if (request.WreckType == WreckType.Scan)
						continue;
					_destroyQueue.Add (data.GetHashCode (), data);
				}
			}

			foreach (InteractableVehicle vehicle in VehicleManager.vehicles)
			{
				if (Vector3.Distance (request.Position, vehicle.transform.position.ToSystemVector ()) > request.Radius)
					continue;

				if (_destroyQueue.ContainsKey (vehicle.GetHashCode ()))
					continue;

				if (request.SteamID != 0)
				{
					if (vehicle.lockedOwner.m_SteamID != request.SteamID)
						continue;
					if (request.WreckType != WreckType.Scan && _permissionProvider.CheckHasAnyPermission (_wreckPlugin.Container.Resolve<IPlayerManager> ().GetPlayer (request.SteamID.ToString ()), "wreckingball.skip.vehicle", "wreckingball.skip.*", "wreckingball.*") == PermissionResult.Grant)
						continue;
				}

				if (request.ItemID != 0)
					if (vehicle.id != request.ItemID)
						continue;

				if (request.Filters != null)
					if (!_wreckPlugin.ConfigurationInstance.Elements.Any (c => request.Filters.Contains (c.CategoryId) && c.Id == vehicle.id || request.Filters.Contains ('*')))
						continue;

				if (request.WreckType != WreckType.Scan && _permissionProvider.CheckHasAnyPermission (request.User, "wreckingball.skip.vehicle", "wreckingball.skip.*", "wreckingball.*") == PermissionResult.Grant)
					continue;

				objectsFound++;
				if (request.WreckType == WreckType.Scan)
					continue;
				_destroyQueue.Add (vehicle.GetHashCode (), vehicle);
			}

			if (request.Filters.Contains ('A'))
			{
				foreach (Animal animal in AnimalManager.animals)
				{
					if (Vector3.Distance (request.Position, animal.transform.position.ToSystemVector ()) > request.Radius)
						continue;

					if (_destroyQueue.ContainsKey (animal.GetHashCode ()))
						continue;

					if (request.ItemID != 0)
						if (animal.id != request.ItemID)
							continue;

					objectsFound++;
					if (request.WreckType == WreckType.Scan)
						continue;
					_destroyQueue.Add (animal.GetHashCode (), animal);
				}
			}
			if (request.Filters.Contains ('Z'))
			{
				foreach (ZombieRegion region in ZombieManager.regions)
				{
					foreach (Zombie zombie in region.zombies)
					{
						if (Vector3.Distance (request.Position, zombie.transform.position.ToSystemVector ()) > request.Radius)
							continue;

						if (_destroyQueue.ContainsKey (zombie.GetHashCode ()))
							continue;

						if (request.ItemID != 0)
							if (zombie.id != request.ItemID)
								continue;

						objectsFound++;
						if (request.WreckType == WreckType.Scan)
							continue;
						_destroyQueue.Add (zombie.GetHashCode (), zombie);
					}
				}
			}

			request.User.SendLocalizedMessage (_wreckPlugin.Translations, "wreckingball_scan", objectsFound);
			_scanQueue.Remove (request);
			if (request.WreckType == WreckType.Scan)
				return;
			request.User.SendLocalizedMessage (_wreckPlugin.Translations, "wreckingball_added_destruction", FormattedTimeUntilDestroyed ());
		}

		public void DestructionRun ()
		{
			if (_destroyQueue.Count == 0)
				return;
			if ((DateTime.Now - _lastDestructionRun).TotalMilliseconds <= (1000/_wreckPlugin.ConfigurationInstance.DestructionInterval) &&
				_amountDestroyed >= _wreckPlugin.ConfigurationInstance.DestructionsPerInterval)
				return;

			KeyValuePair<int, object> nextDestroy = _destroyQueue.First ();

			if (nextDestroy.Value is BarricadeData bData)
			{
				Transform transform = Physics.OverlapSphere (bData.point, 0.5f, 1 << LayerMasks.BARRICADE).FirstOrDefault (c => c.transform.position == bData.point)?.transform;
				if (transform == null)
					return;
				BarricadeManager.damage (transform, ushort.MaxValue, 1, false);
			}
			if (nextDestroy.Value is StructureData sData)
			{
				Transform transform = Physics.OverlapSphere (sData.point, 0.5f, 1 << LayerMasks.BARRICADE).FirstOrDefault (c => c.transform.position == sData.point)?.transform;
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
				aData.askDamage (byte.MaxValue, aData.transform.up, out EPlayerKill _, out uint _);
			}
			if (nextDestroy.Value is Zombie zData)
			{
				zData.askDamage (byte.MaxValue, zData.transform.up, out EPlayerKill _, out uint _);
			}

			_destroyQueue.Remove (nextDestroy.Key);
			_amountDestroyed++;
			if (_amountDestroyed == _wreckPlugin.ConfigurationInstance.DestructionsPerInterval)
				_lastDestructionRun = DateTime.Now;
		}

		private DateTime _lastVehicleCapRun;
		public void VehicleCapRun ()
		{
			if ((DateTime.Now - _lastVehicleCapRun).TotalSeconds < _wreckPlugin.ConfigurationInstance.VehicleDestructionInterval)
				return;
			int runCount = 0;

			while (VehicleManager.vehicles.Count > _wreckPlugin.ConfigurationInstance.MaxVehiclesAllowed)
			{
				InteractableVehicle vehicle = VehicleManager.vehicles.FirstOrDefault ();

				if (vehicle == null)
				{
					_lastVehicleCapRun = DateTime.Now;
					return;
				}

				if (vehicle.isLocked && _permissionProvider.CheckHasAnyPermission (_wreckPlugin.Container.Resolve <IPlayerManager> ().GetPlayer (vehicle.lockedOwner.m_SteamID.ToString ()), "wreck.skip.vehicle", "wreck.skip.*", "wreck.*") == PermissionResult.Grant)
				{
					_lastVehicleCapRun = DateTime.Now;
					return;
				}

				bool skip = false;
				int count = 0;

				BarricadeManager.tryGetPlant (vehicle.transform, out byte _, out byte _, out ushort _, out BarricadeRegion region);
				count += region.barricades.Count;

				if (_wreckPlugin.ConfigurationInstance.KeepVehiclesWithSigns && region.drops.Any (c => c.interactable is InteractableSign sign && sign.text.Contains (_wreckPlugin.ConfigurationInstance.VehicleSignFlag)))
					continue;

				foreach (var tranCar in vehicle.trainCars)
				{
					BarricadeManager.tryGetPlant (tranCar.root, out _, out _, out _, out BarricadeRegion tRegion);
					count += tRegion.barricades.Count;
					if (_wreckPlugin.ConfigurationInstance.KeepVehiclesWithSigns && tRegion.drops.Any (c => c.interactable is InteractableSign sign && sign.text.Contains (_wreckPlugin.ConfigurationInstance.VehicleSignFlag)))
					{
						skip = true;
						break;
					}
				}

				if (_wreckPlugin.ConfigurationInstance.LowElementCountOnly)
					if (count >= _wreckPlugin.ConfigurationInstance.MinElementCount)
						skip = true;

				if (!skip)
					vehicle.askDamage (ushort.MaxValue, true);

				runCount++;
				if (runCount > 15)
					break;
			}

			_lastVehicleCapRun = DateTime.Now;
		}

		public string FormattedTimeUntilDestroyed ()
		{
			float time = _wreckPlugin.ConfigurationInstance.DestructionsPerInterval * (float) (_destroyQueue.Count / _wreckPlugin.ConfigurationInstance.DestructionsPerInterval);

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
}
