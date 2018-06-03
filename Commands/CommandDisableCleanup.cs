using System;
using Rocket.API.Commands;
using Rocket.API.Plugins;
using Rocket.Core.User;
using Rocket.API.I18N;
using Rocket.Unturned.Player;
using Rocket.API.User;
using Rocket.API.Player;

namespace WreckingBall
{
	public class CommandDisableCleanup : ICommand
	{
		private WreckingBallPlugin wreckPlugin;

		public string Name => "disablecleanup";
		public string [] Aliases => new string []
		{
			"disablec"
		};
		public string Summary => "Disables cleanup on player";
		public string Description => throw new NotImplementedException ();
		public string Permission => null;
		public string Syntax => "<Player Name | Steam ID>";
		public IChildCommand [] ChildCommands => null;

		public CommandDisableCleanup (WreckingBallPlugin plugin)
		{
			wreckPlugin = plugin;
		}

		public void Execute (ICommandContext context)
		{
			throw new NotImplementedException ();
		}

		public bool SupportsUser (Type user) => true;
	}
}
