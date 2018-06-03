using Rocket.API.Commands;
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
		public IChildCommand [] ChildCommands => new IChildCommand []
		{

		};

		public CommandScan (WreckingBallPlugin plugin)
		{
			this.wreckPlugin = plugin;
		}

		public void Execute (ICommandContext context)
		{
			if (!(caller.HasPermission ("wreck.scan") || caller.HasPermission ("wreck.*")) && !(caller is ConsolePlayer))
			{
				UnturnedChat.Say (caller, WreckingBall.Instance.Translate ("wreckingball_scan_permission"), Color.red);
				return;
			}
			if ((oper.Length == 3 && !(caller is ConsolePlayer)) || (oper.Length == 6 && caller is ConsolePlayer))
			{
				if (caller is ConsolePlayer)
				{
					if (!cmd.GetVectorFromCmd (3, out position))
					{
						UnturnedChat.Say (caller, WreckingBall.Instance.Translate ("wreckingball_help_scan_console"));
						break;
					}
				}
				ushort itemID = 0;
				if (ushort.TryParse (oper [1], out itemID))
					WreckingBall.Instance.Scan (caller, oper [1], Convert.ToUInt32 (oper [2]), position, FlagType.ItemID, 0, itemID);
				else
					WreckingBall.Instance.Scan (caller, oper [1], Convert.ToUInt32 (oper [2]), position, FlagType.Normal, 0, 0);
			}
			else if ((oper.Length == 4 && !(caller is ConsolePlayer)) || (oper.Length == 7 && caller is ConsolePlayer))
			{
				ulong steamID = 0;
				if (caller is ConsolePlayer)
				{
					if (!cmd.GetVectorFromCmd (4, out position))
					{
						UnturnedChat.Say (caller, WreckingBall.Instance.Translate ("wreckingball_help_scan_console"));
						break;
					}
				}
				if (oper [1].isCSteamID (out steamID))
					WreckingBall.Instance.Scan (caller, oper [2], Convert.ToUInt32 (oper [3]), position, FlagType.SteamID, (ulong) steamID, 0);
				else
					UnturnedChat.Say (caller, WreckingBall.Instance.Translate ("wreckingball_help_scan"));
			}
			else
			{
				UnturnedChat.Say (caller, WreckingBall.Instance.Translate ("wreckingball_help_scan"));
			}
		}

		public bool SupportsUser (Type user) => true;
	}
}
