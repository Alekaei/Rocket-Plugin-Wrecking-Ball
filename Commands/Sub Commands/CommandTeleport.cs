using Rocket.API.Commands;
using System;

namespace WreckingBall
{
	public class CommandTeleport : IChildCommand
	{
		private WreckingBallPlugin wreckPlugin;

		public string Name => "Teleport";
		public string [] Aliases => new string []
		{
			"t"
		};
		public string Summary => "Base wreck command";
		public string Description => throw new NotImplementedException ();
		public string Permission => null;
		public string Syntax => "<Scan | Destroy | Teleport | List>";
		public IChildCommand [] ChildCommands => new IChildCommand []
		{

		};

		public CommandTeleport (WreckingBallPlugin plugin)
		{
			this.wreckPlugin = plugin;
		}

		public void Execute (ICommandContext context)
		{
			if (!(caller.HasPermission ("wreck.teleport") || caller.HasPermission ("wreck.*")) && !(caller is ConsolePlayer))
			{
				UnturnedChat.Say (caller, WreckingBall.Instance.Translate ("wreckingball_teleport_permission"), Color.red);
				return;
			}
			if (oper.Length > 1)
			{
				if (caller is ConsolePlayer)
				{
					UnturnedChat.Say (caller, WreckingBall.Instance.Translate ("wreckingball_teleport_not_allowed"));
					break;
				}
				switch (oper [1])
				{
					case "b":
						WreckingBall.Instance.Teleport (player, TeleportType.Barricades);
						break;
					case "s":
						WreckingBall.Instance.Teleport (player, TeleportType.Structures);
						break;
					case "v":
						WreckingBall.Instance.Teleport (player, TeleportType.Vehicles);
						break;
					default:
						UnturnedChat.Say (caller, WreckingBall.Instance.Translate ("wreckingball_help_teleport"));
						break;
				}
			}
			else
			{
				UnturnedChat.Say (caller, WreckingBall.Instance.Translate ("wreckingball_help_teleport"));
				break;
			}
		}

		public bool SupportsUser (Type user) => true;
	}
}
