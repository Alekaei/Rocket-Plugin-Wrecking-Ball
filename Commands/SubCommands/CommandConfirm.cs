using System;
using Rocket.API.Commands;
using Rocket.API.Plugins;
using Rocket.Unturned.Player;

namespace WreckingBall.Commands.SubCommands
{
	public class CommandConfirm : IChildCommand
	{
		private readonly WreckingBallPlugin _wreckPlugin;

		public string Name => "Confirm";
		public string [] Aliases => null;
		public string Summary => "Confirm last destruction.";
		public string Description => null;
		public string Permission => null;
		public string Syntax => "";
		public IChildCommand [] ChildCommands => null;

		public CommandConfirm (IPlugin plugin)
		{
			_wreckPlugin = (WreckingBallPlugin) plugin;
		}

		public void Execute (ICommandContext context)
		{
			_wreckPlugin.DestructionHandler.ConfirmRequest (context.User);
		}

		public bool SupportsUser (Type user) => typeof (UnturnedUser).IsAssignableFrom (user);
	}
}
