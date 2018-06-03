using Rocket.API.Commands;
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
		public string Summary => "Base wreck command";
		public string Description => throw new NotImplementedException ();
		public string Permission => null;
		public string Syntax => "<Scan | Destroy | Teleport | List>";
		public IChildCommand [] ChildCommands => new IChildCommand []
		{

		};

		public CommandList (WreckingBallPlugin plugin)
		{
			this.wreckPlugin = plugin;
		}

		public void Execute (ICommandContext context)
		{
			throw new NotImplementedException ();
		}

		public bool SupportsUser (Type user) => true;
	}
}
