using Rocket.API.DependencyInjection;
using Rocket.API.User;
using Rocket.Core.I18N;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WreckingBall
{
	public class WreckingBallPlugin : Plugin<Config>
	{
		public static ElementDataManager ElementData;
		private IUserManager userManager;

		public readonly DestructionHandler DestructionHandler;

		public override Dictionary<string, string> DefaultTranslations => new Dictionary<string, string> ()
		{
			{ "wreckingball_lv_help", "<radius> - distance to scan cars." },
			{ "wreckingball_lv2_vehicle", "Vehicle: {5}({6}) position: {0}, with InstanceID: {1}, Barricade count on car: {2}, Sign By: {3}, Locked By: {4}." },
			{ "wreckingball_lv2_traincar", "Vehicle: Train Car#{5} position: {0}, with InstanceID: {1}, Barricade count on Train Car: {2}, Sign By: {3}, Locked By: {4}." },
			{ "wreckingball_dcu_help", "<\"playername\" | SteamID> - disables cleanup on a player." },
			{ "wreckingball_dcu_not_enabled", "This command can only be used if the cleanup feature is enabled on the server." },
			{ "wreckingball_dcu_player_not_found", "Couldn't find a player by that name on the server." },
			{ "wreckingball_dcu_hasnt_played", "Player hasn't played on this server yet." },
			{ "wreckingball_dcu_cleanup_disabled", "Auto Cleanup has been disabled for player {0} [{1}] ({2})" },
			{ "wreckingball_dcu_cleanup_enabled", "Auto Cleanup has been enabled for player {0} [{1}] ({2})" },

			{ "wreckingball_scan", "Found {0} elements of type: {1}, @ {2}m:{3}" },

			{ "wreckingball_map_clear", "Map has no elements!" },
			{ "wreckingball_not_found", "No elements found in a {0} radius!" },
			{ "wreckingball_complete", "Wrecking Ball complete! {0} elements(s) Destroyed!" },
			{ "wreckingball_initiated", "Wrecking Ball initiated: ~{0} sec(s) left." },

			{ "wreckingball_processing", "Wrecking Ball started by: {0}, {1} element(s) left to destroy, ~{2} sec(s) left." },
			{ "wreckingball_aborted", "Wrecking Ball Aborted! Destruction queue cleared!" },

			{ "wreckingball_queued", "{0} elements(s) found, ~{1} sec(s) to complete run." },
			{ "wreckingball_prompt", "Type '<color=green>/wreck confirm</color>' or '<color=red>/wreck abort</color>'" },

			{ "wreckingball_structure_array_sync_error", "Warning: Structure arrays out of sync, need to restart server." },
			{ "wreckingball_barricade_array_sync_error", "Warning: Barricade arrays out of sync, need to restart server." },
			{ "wreckingball_sync_error", "Warning: Element array sync error, not all elements will be cleaned up in range, server should be restarted." },
			{ "wreckingball_teleport_not_found", "Couldn't find any elements to teleport to, try to run the command again." },
			{ "wreckingball_teleport_not_allowed", "Not allowed to use wreck teleport from the console." },
			{ "wreckingball_reload_abort", "Warning: Current wreck job in progress has been aborted from a plugin reload." },
			{ "wreckingball_wreck_permission", "You need to have the permissions wreck.wreck, or wreck.* to be able to run a wreck." },
			{ "wreckingball_scan_permission", "You need to have the permissions wreck.scan, or wreck.* to be able to run a scan." },
			{ "wreckingball_teleport_permission", "You need to have the permissions wreck.teleport, or wreck.* to be able to run a teleport." }
		};

		protected WreckingBallPlugin (
			IDependencyContainer container,
			IUserManager userManager
			) : base ("Wrecking Ball II", container)
		{
			this.userManager = userManager;
		}

		protected override void OnLoad (bool isFromReload)
		{
			ElementData = new ElementDataManager (this);
			if (ConfigurationInstance.DestructionRate <= 0)
			{
				ConfigurationInstance.DestructionRate = 1;
				Logger.LogWarning ("Error: DestructionRate config value must be above 0.");
			}
			if (ConfigurationInstance.DestructionsPerInterval < 1)
			{
				ConfigurationInstance.DestructionsPerInterval = 1;
				Logger.LogWarning ("Error: DestructionsPerInterval config value must be at or above 1.");
			}
			Configuration.Save ();
		}

		protected override void OnUnload ()
		{
			if (DestructionProcessing.processing)
			{
				if (DestructionProcessing.originalCaller != null)
					userManager.BroadcastLocalized (Translations, "wreckingball_reload_abort", System.Drawing.Color.Yellow);
				DestructionProcessing.Abort (WreckType.Wreck);
			}
			if (DestructionProcessing.cleanupProcessingBuildables || DestructionProcessing.cleanupProcessingFiles)
			{
				DestructionProcessing.Abort (WreckType.Cleanup);
			}
			ElementData = null;
		}

		internal void Scan (IRocketPlayer caller, string filter, uint radius, Vector3 position, FlagType flagType, ulong steamID, ushort itemID)
		{
			DestructionProcessing.Wreck (caller, filter, radius, position, WreckType.Scan, flagType, steamID, itemID);
			if (ElementData.reportLists [BuildableType.Element].Count > 0 || ElementData.reportLists [BuildableType.VehicleElement].Count > 0)
			{
				foreach (KeyValuePair<BuildableType, Dictionary<char, uint>> reportDictionary in ElementData.reportLists)
				{
					if (reportDictionary.Value.Count == 0)
						continue;
					string report = "";
					uint totalCount = 0;
					foreach (KeyValuePair<char, uint> reportFilter in reportDictionary.Value)
					{
						report += " " + ElementData.categorys [reportFilter.Key].Name + ": " + reportFilter.Value + ",";
						totalCount += reportFilter.Value;
					}
					if (report != "")
						report = report.Remove (report.Length - 1);
					string type = reportDictionary.Key == BuildableType.VehicleElement ? "Vehicle Element" : "Element";
					UnturnedChat.Say (caller, Translate ("wreckingball_scan", totalCount, type, radius, report));
					if (ConfigurationInstance.LogScans && !(caller is ConsolePlayer))
						Logger.Log (Translate ("wreckingball_scan", totalCount, type, radius, report));
				}
			}
			else
			{
				UnturnedChat.Say (caller, Translate ("wreckingball_not_found", radius));
			}



		}

		internal void Teleport (IRocketPlayer caller, TeleportType teleportType)
		{

			if (StructureManager.regions.LongLength == 0 && BarricadeManager.BarricadeRegions.LongLength == 0)
			{
				UnturnedChat.Say (caller, Translate ("wreckingball_map_clear"));
				return;
			}

			UnturnedPlayer player = (UnturnedPlayer) caller;

			Vector3 tpVector;
			bool match = false;
			int tries = 0;

			Transform current = null;

			while (tries < 2000 && !match)
			{
				tries++;
				int x = 0;
				int xCount = 0;
				int z = 0;
				int zCount = 0;
				int idx = 0;
				int idxCount = 0;
				switch (teleportType)
				{
					case TeleportType.Structures:
						xCount = StructureManager.regions.GetLength (0);
						zCount = StructureManager.regions.GetLength (1);
						if (xCount == 0)
							continue;
						x = UnityEngine.Random.Range (0, xCount - 1);
						if (zCount == 0)
							continue;
						z = UnityEngine.Random.Range (0, zCount - 1);
						idxCount = StructureManager.regions [x, z].structures.Count;
						if (idxCount == 0)
							continue;
						idx = UnityEngine.Random.Range (0, idxCount - 1);

						try
						{
							current = StructureManager.regions [x, z].drops [idx].model;
						}
						catch
						{
							continue;
						}

						if (Vector3.Distance (current.position, player.Position) > 20)
							match = true;
						break;
					case TeleportType.Barricades:
						xCount = BarricadeManager.BarricadeRegions.GetLength (0);
						zCount = BarricadeManager.BarricadeRegions.GetLength (1);
						if (xCount == 0)
							continue;
						x = UnityEngine.Random.Range (0, xCount - 1);
						if (zCount == 0)
							continue;
						z = UnityEngine.Random.Range (0, zCount - 1);
						idxCount = BarricadeManager.BarricadeRegions [x, z].drops.Count;
						if (idxCount == 0)
							continue;
						idx = UnityEngine.Random.Range (0, idxCount - 1);

						try
						{
							current = BarricadeManager.BarricadeRegions [x, z].drops [idx].model;
						}
						catch
						{
							continue;
						}

						if (Vector3.Distance (current.position, player.Position) > 20)
							match = true;
						break;
					case TeleportType.Vehicles:
						int vCount = VehicleManager.vehicles.Count;
						int vRand = UnityEngine.Random.Range (0, vCount - 1);
						try
						{
							current = VehicleManager.vehicles [vRand].transform;
						}
						catch
						{
							continue;
						}
						if (Vector3.Distance (current.position, player.Position) > 20)
							match = true;
						break;
					default:
						return;
				}
			}
			if (match)
			{
				tpVector = new Vector3 (current.position.x, teleportType == TeleportType.Vehicles ? current.position.y + 4 : current.position.y + 2, current.position.z);
				player.Teleport (tpVector, player.Rotation);
				return;
			}
			UnturnedChat.Say (caller, Translate ("wreckingball_teleport_not_found"));
		}

		internal void Instruct (IRocketPlayer caller)
		{
			UnturnedChat.Say (caller, Translate ("wreckingball_queued", DestructionProcessing.dIdxCount, DestructionProcessing.CalcProcessTime ()));
			if (DestructionProcessing.syncError)
				UnturnedChat.Say (caller, Translate ("wreckingball_sync_error"));
			UnturnedChat.Say (caller, Translate ("wreckingball_prompt"));
		}

		internal void Confirm (IRocketPlayer caller)
		{
			if (DestructionProcessing.destroyList.Count <= 0)
			{
				UnturnedChat.Say (caller, Instance.Translate ("wreckingball_help"));
			}
			else
			{
				DestructionProcessing.processing = true;
				if (!(caller is ConsolePlayer))
					DestructionProcessing.originalCaller = (UnturnedPlayer) caller;
				UnturnedChat.Say (caller, Translate ("wreckingball_initiated", DestructionProcessing.CalcProcessTime ()));
				Logger.Log (string.Format ("Player {0} has initiated wreck.", caller is ConsolePlayer ? "Console" : ((UnturnedPlayer) caller).CharacterName + " [" + ((UnturnedPlayer) caller).SteamName + "] (" + ((UnturnedPlayer) caller).CSteamID.ToString () + ")"));
				DestructionProcessing.dIdxCount = DestructionProcessing.destroyList.Count;
				DestructionProcessing.dIdx = 0;
			}
		}
		
		public void WreckDestroyer ()
		{

		}

		public void VehicleCap ()
		{

		}
	}
}
