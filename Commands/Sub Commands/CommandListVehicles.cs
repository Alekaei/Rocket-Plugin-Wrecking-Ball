using System;
using System.Numerics;
using Rocket.API.Commands;
using Rocket.API.Player;
using Rocket.Core.I18N;
using Rocket.UnityEngine.Extensions;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace WreckingBall
{
	public class CommandListVehicles : IChildCommand
	{
		private WreckingBallPlugin wreckPlugin;

		public string Name => "vehicles";
		public string [] Aliases => new string []
		{
			"v"
		};
		public string Summary => "List vehicles barricade counts";
		public string Description => throw new NotImplementedException ();
		public string Permission => null;
		public string Syntax => "<radius>";
		public IChildCommand [] ChildCommands => new IChildCommand [0];

		public CommandListVehicles (WreckingBallPlugin plugin)
		{
			this.wreckPlugin = plugin;
		}

		public void Execute (ICommandContext context)
		{
			UnturnedPlayer player = ((UnturnedUser) wreckPlugin.Container.Resolve<IPlayerManager> ().GetOnlinePlayerById (context.User.Id)).Player;
			int radius = context.Parameters.Get<int> (0);

			foreach (var vehicle in VehicleManager.vehicles)
			{
				if (Vector3.Distance (player.Entity.Position, vehicle.transform.position.ToSystemVector ()) > radius)
					continue;
				int count = 0;
				BarricadeManager.tryGetPlant (vehicle.transform, out byte x, out byte y, out ushort plant, out BarricadeRegion region);
				count += region.barricades.Count;

				foreach (var trainCar in vehicle.trainCars)
				{
					BarricadeManager.tryGetPlant (trainCar.root, out x, out y, out plant, out BarricadeRegion tRegion);
					count += tRegion.barricades.Count;
				}

				context.User.SendLocalizedMessage (
					wreckPlugin.Translations, 
					"wreckingball_list_v", 
					vehicle.asset.vehicleName, 
					vehicle.transform.position.ToString (), 
					count);
			}
		}

		public bool SupportsUser (Type user) => typeof (UnturnedUser).IsAssignableFrom (user);
	}
}
