using System;
using Rocket.API.Commands;
using Rocket.API.Plugins;
using Rocket.Core.Commands;
using Rocket.Unturned.Player;

namespace WreckingBall.Commands.SubCommands
{
	public class CommandList : IChildCommand
	{
		private readonly WreckingBallPlugin _wreckPlugin;

		public string Name => "List";
		public string [] Aliases => new[]
		{
			"l"
		};
		public string Summary => "List Top Player|Vehicles";
		public string Description => null;
		public string Permission => null;
		public string Syntax => "<topplayers|vehicles>";

		public IChildCommand [] ChildCommands => new IChildCommand []
		{
			new CommandListTopPlayers (_wreckPlugin),
			new CommandListVehicles (_wreckPlugin)
		};

		public CommandList (IPlugin plugin)
		{
			_wreckPlugin = (WreckingBallPlugin) plugin;
		}

		public void Execute (ICommandContext context)
		{
			throw new CommandWrongUsageException ();
		}

		public bool SupportsUser (Type user) => typeof (UnturnedUser).IsAssignableFrom (user);
	}
}
