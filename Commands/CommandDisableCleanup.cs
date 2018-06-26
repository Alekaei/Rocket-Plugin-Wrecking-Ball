using System;
using Rocket.API.Commands;

namespace WreckingBall.Commands
{
	public class CommandDisableCleanup : ICommand
	{
		public string Name => "disablecleanup";
		public string [] Aliases => new[]
		{
			"disablec"
		};
		public string Summary => "Disables cleanup on player";
		public string Description => null;
		public string Permission => null;
		public string Syntax => "<Player Name | Steam ID>";
		public IChildCommand [] ChildCommands => null;

		public void Execute (ICommandContext context)
		{
			throw new NotImplementedException ();
		}

		public bool SupportsUser (Type user) => true;
	}
}
