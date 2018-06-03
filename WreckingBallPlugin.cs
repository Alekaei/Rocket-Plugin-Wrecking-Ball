using Rocket.API.DependencyInjection;
using Rocket.API.Scheduler;
using Rocket.API.User;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using System.Collections.Generic;

namespace WreckingBall
{
	public class WreckingBallPlugin : Plugin<Config>
	{
		private IUserManager userManager;
		private ITaskScheduler taskScheduler;

		public readonly DestructionHandler DestructionHandler;

		public override Dictionary<string, string> DefaultTranslations => new Dictionary<string, string> ()
		{
			{ "wreckingball_dcu_not_enabled", "This command can only be used if the cleanup feature is enabled on the server." },
			{ "wreckingball_dcu_player_not_found", "Couldn't find a player by that name on the server." },
			{ "wreckingball_dcu_hasnt_played", "Player hasn't played on this server yet." },
			{ "wreckingball_dcu_cleanup_disabled", "Auto Cleanup has been disabled for player {0} [{1}] ({2})" },
			{ "wreckingball_dcu_cleanup_enabled", "Auto Cleanup has been enabled for player {0} [{1}] ({2})" },
			// Renewed
			{ "wreckingball_prompt", "Type '<color=green>/wreck confirm</color>' or '<color=red>/wreck abort</color>'." },

			{ "wreckingball_confirmed", "Confirmed your last wreck request, calculating objects." },
			{ "wreckingball_aborted", "Your last wreck request has been aborted." },
			{ "wreckingball_no_request", "You currently don't have a pending wreck request!" },

			{ "wreckingball_scan", "Found {0} objects from your last request." },
			{ "wreckingball_added_destruction", "Added above objects to <color=red>destruction</color> queue. Estimated time: {0}" },

			{ "wreckingball_list_v", "[{0}] at position: {1} with {2} barricades." },
			{ "wreckingball_list_tp", "[#{0}] Steam ID: {0} Object count: {1}." },
		};

		protected WreckingBallPlugin (
			IDependencyContainer container,
			IUserManager userManager,
			ITaskScheduler taskScheduler
			) : base ("WreckingBallII", container)
		{
			this.userManager = userManager;
			this.taskScheduler = taskScheduler;
			DestructionHandler = new DestructionHandler (this, taskScheduler);
		}

		protected override void OnLoad (bool isFromReload)
		{
			DestructionHandler.Load ();

			if (ConfigurationInstance.DestructionRate <= 0)
			{
				ConfigurationInstance.DestructionRate = 1;
				Logger.LogWarning ("DestructionRate config value must be at or above 1.");
			}
			if (ConfigurationInstance.DestructionsPerInterval < 1)
			{
				ConfigurationInstance.DestructionsPerInterval = 1;
				Logger.LogWarning ("DestructionsPerInterval config value must be at or above 1.");
			}
			Configuration.Save ();
		}

		protected override void OnUnload ()
		{
			
		}
	}
}
