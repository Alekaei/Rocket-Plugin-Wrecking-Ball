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
			if (!(caller.HasPermission ("wreck.wreck") || caller.HasPermission ("wreck.*")) && !(caller is ConsolePlayer))
			{
				UnturnedChat.Say (caller, WreckingBall.Instance.Translate ("wreckingball_wreck_permission"), Color.red);
				return;
			}
			if (!(caller is ConsolePlayer))
				UnturnedChat.Say (caller, WreckingBall.Instance.Translate ("wreckingball_aborted"));
			Logger.Log (WreckingBall.Instance.Translate ("wreckingball_aborted"));
			DestructionProcessing.Abort (WreckType.Wreck);
		}

		public bool SupportsUser (Type user) => true;
	}
}
