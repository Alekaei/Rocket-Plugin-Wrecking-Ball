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
			if (!wreckPlugin.ConfigurationInstance.EnableCleanup)
			{
				context.User.SendMessage (wreckPlugin.Translations.Get ("wreckingball_dcu_not_enabled"), System.Drawing.Color.Red);
				return;
			}
			
			IPlayer user = context.Parameters.Get<IPlayer> (0);
			if (user == null)
			{
				throw new PlayerNotOnlineException ();
			}
			UnturnedPlayer player = ((UnturnedUser) user)?.Player;
			if (player == null)
			{
				throw new PlayerNotOnlineException ();
			}
			/*
			if (IsPInfoLibLoaded ())
			{
				PlayerData pData = PlayerInfoLib.Database.QueryById ((CSteamID) steamID, false);
				if (!pData.IsLocal ())
				{
					UnturnedChat.Say (caller, Translate ("wreckingball_dcu_hasnt_played"), Color.red);
					return;
				}
				if (pData.CleanedBuildables && pData.CleanedPlayerData)
				{
					PlayerInfoLib.Database.SetOption (pData.SteamID, OptionType.Buildables, false);
					PlayerInfoLib.Database.SetOption (pData.SteamID, OptionType.PlayerFiles, false);
					UnturnedChat.Say (caller, Translate ("wreckingball_dcu_cleanup_enabled", pData.CharacterName, pData.SteamName, pData.SteamID));

				}
				else
				{
					PlayerInfoLib.Database.SetOption (pData.SteamID, OptionType.Buildables, true);
					PlayerInfoLib.Database.SetOption (pData.SteamID, OptionType.PlayerFiles, true);
					UnturnedChat.Say (caller, Translate ("wreckingball_dcu_cleanup_disabled", pData.CharacterName, pData.SteamName, pData.SteamID));
				}
			}
			else
			{
				UnturnedChat.Say (caller, Translate ("werckingball_dcu_not_enabled"), Color.red);
			}
			*/
		}

		public bool SupportsUser (Type user) => true;
	}
}
