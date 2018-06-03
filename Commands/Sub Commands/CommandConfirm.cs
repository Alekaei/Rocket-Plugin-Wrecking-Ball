using Rocket.API.Commands;
using Rocket.Unturned.Player;
using System;

namespace WreckingBall
{
	public class CommandConfirm : IChildCommand
	{
		private WreckingBallPlugin wreckPlugin;

		public string Name => "Confirm";
		public string [] Aliases => new string [0];
		public string Summary => "Confirm last destruction.";
		public string Description => throw new NotImplementedException ();
		public string Permission => null;
		public string Syntax => "";
		public IChildCommand [] ChildCommands => new IChildCommand [0];

		public CommandConfirm (WreckingBallPlugin plugin)
		{
			this.wreckPlugin = plugin;
		}

		public void Execute (ICommandContext context)
		{
			wreckPlugin.DestructionHandler.ConfirmRequest (context.User);
		}

		public bool SupportsUser (Type user) => typeof (UnturnedUser).IsAssignableFrom (user);
	}
}
