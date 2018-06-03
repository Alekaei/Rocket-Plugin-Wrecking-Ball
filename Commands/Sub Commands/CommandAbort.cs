using Rocket.API.Commands;
using System;

namespace WreckingBall
{
	public class CommandAbort : IChildCommand
	{
		private WreckingBallPlugin wreckPlugin;

		public string Name => "Abort";
		public string [] Aliases => new string []
		{
			"l"
		};
		public string Summary => "Abort last destruction.";
		public string Description => throw new NotImplementedException ();
		public string Permission => null;
		public string Syntax => "";
		public IChildCommand [] ChildCommands => new IChildCommand [0];

		public CommandAbort (WreckingBallPlugin plugin)
		{
			this.wreckPlugin = plugin;
		}

		public void Execute (ICommandContext context)
		{
			wreckPlugin.DestructionHandler.AbortRequest (context.User);
		}

		public bool SupportsUser (Type user) => true;
	}
}
