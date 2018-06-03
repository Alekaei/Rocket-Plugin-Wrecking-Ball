using Rocket.API.Commands;
using Rocket.Core.Commands;
using Rocket.Unturned.Player;
using System;

namespace WreckingBall
{
	public class CommandList : IChildCommand
	{
		private WreckingBallPlugin wreckPlugin;

		public string Name => "List";
		public string [] Aliases => new string []
		{
			"l"
		};
		public string Summary => "List Top Player|Vehicles";
		public string Description => throw new NotImplementedException ();
		public string Permission => null;
		public string Syntax => "<topplayers|vehicles>";

		public IChildCommand [] ChildCommands => new IChildCommand []
		{
			new CommandListTopPlayers (wreckPlugin),
			new CommandListVehicles (wreckPlugin)
		};

		public CommandList (WreckingBallPlugin plugin)
		{
			this.wreckPlugin = plugin;
		}

		public void Execute (ICommandContext context)
		{
			throw new CommandWrongUsageException ();
		}

		public bool SupportsUser (Type user) => typeof (UnturnedUser).IsAssignableFrom (user);
	}
}
