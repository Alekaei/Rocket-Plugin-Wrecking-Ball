using System;
using System.Numerics;
using Rocket.API.Commands;
using Rocket.API.Plugins;
using Rocket.Core.I18N;
using Rocket.UnityEngine.Extensions;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace WreckingBall.Commands.SubCommands
{
	public class CommandListVehicles : IChildCommand
	{
		private readonly WreckingBallPlugin _wreckPlugin;

		public string Name => "vehicles";
		public string [] Aliases => new[]
		{
			"v"
		};
		public string Summary => "List vehicles barricade counts";
	    public string Description => null;
		public string Permission => null;
		public string Syntax => "<radius>";
	    public IChildCommand[] ChildCommands => null;

		public CommandListVehicles (IPlugin plugin)
		{
			_wreckPlugin = (WreckingBallPlugin) plugin;
		}

		public void Execute (ICommandContext context)
		{
		    UnturnedPlayer player = ((UnturnedUser) context.User).Player;

			int radius = context.Parameters.Get<int> (0);

			foreach (var vehicle in VehicleManager.vehicles)
			{
				if (Vector3.Distance (player.Entity.Position, vehicle.transform.position.ToSystemVector ()) > radius)
					continue;
				int count = 0;
				BarricadeManager.tryGetPlant (vehicle.transform, out byte _, out byte _, out ushort _, out BarricadeRegion region);
				count += region.barricades.Count;

				foreach (var trainCar in vehicle.trainCars)
				{
					BarricadeManager.tryGetPlant (trainCar.root, out _, out _, out _, out BarricadeRegion tRegion);
					count += tRegion.barricades.Count;
				}

				context.User.SendLocalizedMessage (
					_wreckPlugin.Translations, 
					"wreckingball_list_v", 
					vehicle.asset.vehicleName, 
					vehicle.transform.position.ToString (), 
					count);
			}
		}

		public bool SupportsUser (Type user) => typeof (UnturnedUser).IsAssignableFrom (user);
	}
}
