using System;
using Rocket.API.Commands;
using Rocket.API.Player;
using Rocket.Core.Commands;
using Rocket.Core.I18N;
using Rocket.Unturned.Player;

namespace WreckingBall
{
	public class CommandWreck : ICommand
	{
		private WreckingBallPlugin wreckPlugin;

		public string Name => "Wreck";
		public string [] Aliases => new string []
		{
			"w"
		};
		public string Summary => "Base wreck command";
		public string Description => throw new NotImplementedException ();
		public string Permission => null;
		public string Syntax => "[Scan | Teleport | List | Confirm | Abort] | <Flag | Item ID> <Radius>";
		public IChildCommand [] ChildCommands => new IChildCommand []
		{
			new CommandScan (wreckPlugin),
			new CommandTeleport (wreckPlugin),
			new CommandList (wreckPlugin),

			new CommandConfirm (wreckPlugin),
			new CommandAbort (wreckPlugin)
		};

		public CommandWreck (WreckingBallPlugin plugin)
		{
			this.wreckPlugin = plugin;
		}

		public void Execute (ICommandContext context)
		{
			UnturnedPlayer player = ((UnturnedUser) wreckPlugin.Container.Resolve<IPlayerManager> ().GetOnlinePlayerById (context.User.Id)).Player;

			switch (context.Parameters.Length)
			{
				// Specified Steam ID
				// /wreck {0:steamID} {1:Flag|ItemID} {2:radius}
				case 3:
					ulong steamID = context.Parameters.Get<ulong> (0);
					string flag = (context.Parameters.TryGet <ushort> (1, out ushort itemID)? null : context.Parameters.Get <string> (1));
					uint radius = context.Parameters.Get<uint> (2);

					if (flag == null)
						wreckPlugin.DestructionHandler.AddRequest (player, null, radius, player.Entity.Position, WreckType.Wreck, steamID, itemID);
					else
						wreckPlugin.DestructionHandler.AddRequest (player, flag, radius, player.Entity.Position, WreckType.Wreck, steamID, 0);

					context.User.SendLocalizedMessage (wreckPlugin.Translations, "wreckingball_prompt");
					break;
				// No Steam ID Specified
				case 2:
					flag = (context.Parameters.TryGet<ushort> (1, out itemID) ? null : context.Parameters.Get<string> (1));
					radius = context.Parameters.Get<uint> (2);

					if (flag == null)
						wreckPlugin.DestructionHandler.AddRequest (player, null, radius, player.Entity.Position, WreckType.Wreck, 0, itemID);
					else
						wreckPlugin.DestructionHandler.AddRequest (player, flag, radius, player.Entity.Position, WreckType.Wreck, 0, 0);

					context.User.SendLocalizedMessage (wreckPlugin.Translations, "wreckingball_prompt");
					break;
				default:
					throw new CommandWrongUsageException ();
			}
		}

		public bool SupportsUser (Type user) => typeof (UnturnedUser).IsAssignableFrom (user);
	}
}
