using Rocket.API.Commands;
using Rocket.API.Player;
using Rocket.Core.Commands;
using Rocket.Core.I18N;
using Rocket.Unturned.Player;
using System;

namespace WreckingBall
{
	public class CommandScan : IChildCommand
	{
		private WreckingBallPlugin wreckPlugin;

		public string Name => "Scan";
		public string [] Aliases => new string []
		{
			"s"
		};
		public string Summary => "Scan radius for a certain flag or item id.";
		public string Description => throw new NotImplementedException ();
		public string Permission => null;
		public string Syntax => "[Steam ID] <Flag | Item ID> <Radius>";
		public IChildCommand [] ChildCommands => new IChildCommand [0];

		public CommandScan (WreckingBallPlugin plugin)
		{
			this.wreckPlugin = plugin;
		}

		public void Execute (ICommandContext context)
		{
			UnturnedPlayer player = ((UnturnedUser) wreckPlugin.Container.Resolve<IPlayerManager> ().GetOnlinePlayerById (context.User.Id)).Player;

			switch (context.Parameters.Length)
			{
				// Specified Steam ID
				// /wreck scan {0:steamID} {1:Flag|ItemID} {2:radius}
				case 3:
					ulong steamID = context.Parameters.Get<ulong> (0);
					string flag = (context.Parameters.TryGet<ushort> (1, out ushort itemID) ? null : context.Parameters.Get<string> (1));
					uint radius = context.Parameters.Get<uint> (2);

					if (flag == null)
						wreckPlugin.DestructionHandler.AddRequest (context.User, null, radius, player.Entity.Position, WreckType.Scan, steamID, itemID);
					else
						wreckPlugin.DestructionHandler.AddRequest (context.User, flag, radius, player.Entity.Position, WreckType.Scan, steamID, 0);

					context.User.SendLocalizedMessage (wreckPlugin.Translations, "wreckingball_prompt");
					break;
				// No Steam ID Specified
				case 2:
					flag = (context.Parameters.TryGet<ushort> (1, out itemID) ? null : context.Parameters.Get<string> (1));
					radius = context.Parameters.Get<uint> (2);

					if (flag == null)
						wreckPlugin.DestructionHandler.AddRequest (context.User, null, radius, player.Entity.Position, WreckType.Scan, 0, itemID);
					else
						wreckPlugin.DestructionHandler.AddRequest (context.User, flag, radius, player.Entity.Position, WreckType.Scan, 0, 0);

					context.User.SendLocalizedMessage (wreckPlugin.Translations, "wreckingball_prompt");
					break;
				default:
					throw new CommandWrongUsageException ();
			}
		}

		public bool SupportsUser (Type user) => typeof (UnturnedUser).IsAssignableFrom (user);
	}
}
