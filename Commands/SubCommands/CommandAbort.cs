using System;
using Rocket.API.Commands;
using Rocket.API.Plugins;

namespace WreckingBall.Commands.SubCommands
{
	public class CommandAbort : IChildCommand
	{
		private readonly WreckingBallPlugin _wreckPlugin;

		public string Name => "Abort";
		public string [] Aliases => new[]
		{
			"l"
		};
		public string Summary => "Abort last destruction.";
	    public string Description => null;
		public string Permission => null;
		public string Syntax => "";
	    public IChildCommand[] ChildCommands => null;

		public CommandAbort (IPlugin plugin)
		{
			_wreckPlugin = (WreckingBallPlugin) plugin;
		}

		public void Execute (ICommandContext context)
		{
			_wreckPlugin.DestructionHandler.AbortRequest (context.User);
		}

		public bool SupportsUser (Type user) => true;
	}
}
