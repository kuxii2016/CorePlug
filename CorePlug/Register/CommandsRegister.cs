using Proxyserver.Commands;

namespace Proxyserver.Register
{
    internal class CommandsRegister
    {
        internal static bool RegisterCommands()
        {
            Program.GetPluginManager.CommandManager.RegisterCommand("APP", new PluginsCommand(Program.GetPluginManager));
            Program.GetPluginManager.CommandManager.RegisterCommand("APP", new StartPluginCommand(Program.GetPluginManager));
            Program.GetPluginManager.CommandManager.RegisterCommand("APP", new StopPluginCommand(Program.GetPluginManager));
            Program.GetPluginManager.CommandManager.RegisterCommand("APP", new PluginStatusCommand(Program.GetPluginManager));
            return true;
        }
    }
}