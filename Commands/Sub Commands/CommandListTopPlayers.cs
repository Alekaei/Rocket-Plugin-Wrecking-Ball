using System;
using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Linq;
using Rocket.Core.I18N;

namespace WreckingBall
{
	public class CommandListTopPlayers : IChildCommand
	{
		private WreckingBallPlugin wreckPlugin;

		public string Name => "topplayers";
		public string [] Aliases => new string []
		{
			"tp"
		};
		public string Summary => "List the top players build counts";
		public string Description => throw new NotImplementedException ();
		public string Permission => null;
		public string Syntax => "[max amount]";
		public IChildCommand [] ChildCommands => new IChildCommand [0];

		public CommandListTopPlayers (WreckingBallPlugin plugin)
		{
			this.wreckPlugin = plugin;
		}

		public void Execute (ICommandContext context)
		{
			Dictionary<ulong, int> buildCount = new Dictionary<ulong, int> ();
			foreach (var region in BarricadeManager.regions)
			{
				foreach (var barricade in region.barricades)
				{
					if (buildCount.ContainsKey (barricade.owner))
						buildCount [barricade.owner]++;
					else
						buildCount.Add (barricade.owner, 1);
				}
			}
			foreach (var region in StructureManager.regions)
			{
				foreach (var structure in region.structures)
				{
					if (buildCount.ContainsKey (structure.owner))
						buildCount [structure.owner]++;
					else
						buildCount.Add (structure.owner, 1);
				}
			}
			int maxCount = (context.Parameters.Length > 0) ? context.Parameters.Get<int> (0) : 5;
			for (int i = 0; i < maxCount; i++)
			{
				KeyValuePair<ulong, int> max = buildCount.First (c => c.Value == buildCount.Values.Max ());
				context.User.SendLocalizedMessage (
					wreckPlugin.Translations, 
					"wreckingball_list_tp",
					i+1,
					max.Key,
					max.Value);
				buildCount.Remove (max.Key);
			}
		}

		public bool SupportsUser (Type user) => typeof (UnturnedUser).IsAssignableFrom (user);
	}
}
