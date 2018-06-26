using System;
using Rocket.API.Commands;
using Rocket.API.Plugins;
using Rocket.Core.Commands;
using Rocket.Core.I18N;
using Rocket.Unturned.Player;
using WreckingBall.Commands.SubCommands;
using WreckingBall.Misc;

namespace WreckingBall.Commands
{
    public class CommandWreck : ICommand
    {
        private readonly WreckingBallPlugin _wreckPlugin;

        public string Name => "Wreck";
        public string[] Aliases => new[]
        {
            "w"
        };
        public string Summary => "Base wreck command";
        public string Description => null;
        public string Permission => null;
        public string Syntax => "<Flag | Item ID> <Radius>";
        public IChildCommand[] ChildCommands => new IChildCommand[]
        {
            new CommandScan(_wreckPlugin),
            new CommandTeleport(),
            new CommandList(_wreckPlugin),
            new CommandConfirm(_wreckPlugin),
            new CommandAbort(_wreckPlugin)
        };

        public CommandWreck(IPlugin plugin)
        {
            _wreckPlugin = (WreckingBallPlugin)plugin;
        }

        public void Execute(ICommandContext context)
        {
            UnturnedPlayer player = ((UnturnedUser)context.User).Player;

            switch (context.Parameters.Length)
            {
                // Specified Steam ID
                // /wreck {0:steamID} {1:Flag|ItemID} {2:radius}
                case 3:
                    ulong steamID = context.Parameters.Get<ulong>(0);
                    string flag = (context.Parameters.TryGet(1, out ushort itemID) ? null : context.Parameters.Get<string>(1));
                    uint radius = context.Parameters.Get<uint>(2);

                    if (flag == null)
                        _wreckPlugin.DestructionHandler.AddRequest(context.User, null, radius, player.Entity.Position, WreckType.Wreck, steamID, itemID);
                    else
                        _wreckPlugin.DestructionHandler.AddRequest(context.User, flag, radius, player.Entity.Position, WreckType.Wreck, steamID, 0);

                    context.User.SendLocalizedMessage(_wreckPlugin.Translations, "wreckingball_prompt");
                    break;
                // No Steam ID Specified
                case 2:
                    flag = (context.Parameters.TryGet(1, out itemID) ? null : context.Parameters.Get<string>(1));
                    radius = context.Parameters.Get<uint>(2);

                    if (flag == null)
                        _wreckPlugin.DestructionHandler.AddRequest(context.User, null, radius, player.Entity.Position, WreckType.Wreck, 0, itemID);
                    else
                        _wreckPlugin.DestructionHandler.AddRequest(context.User, flag, radius, player.Entity.Position, WreckType.Wreck, 0, 0);

                    context.User.SendLocalizedMessage(_wreckPlugin.Translations, "wreckingball_prompt");
                    break;
                default:
                    throw new CommandWrongUsageException();
            }
        }

        public bool SupportsUser(Type user) => typeof(UnturnedUser).IsAssignableFrom(user);
    }
}
