using System;
using System.Collections.Generic;
using System.Linq;
using Utils.Logging;

namespace Utils.ServerConsole
{
    public class ConsoleCommandManager
    {
        private readonly List<RegisteredCommand> commands
            = new List<RegisteredCommand>();

        private readonly object sync = new object();

        public void RegisterCommand(string pluginName, IConsoleCommand command)
        {
            lock (sync)
            {
                bool exists = commands.Any(x =>
                    x.Command.Command.Equals(
                        command.Command,
                        StringComparison.OrdinalIgnoreCase));

                if (exists)
                    throw new Exception(
                        $"Command already exists: {command.Command}");

                commands.Add(new RegisteredCommand
                {
                    PluginName = pluginName,
                    Command = command
                });
            }

            Log.Debug(
                $"Console command registered: " +
                $"Plugin='{pluginName}', Command='{command.Command}'");
        }

        public void UnregisterPluginCommands(string pluginName)
        {
            lock (sync)
            {
                commands.RemoveAll(x =>
                    x.PluginName.Equals(
                        pluginName,
                        StringComparison.OrdinalIgnoreCase));
            }

            Log.Info(
                $"All console commands removed for plugin: {pluginName}");
        }

        public bool Execute(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            string[] split = input.Split(' ');

            string commandName = split[0];

            string[] args = split.Skip(1).ToArray();

            RegisteredCommand command;

            lock (sync)
            {
                command = commands.FirstOrDefault(x =>
                    x.Command.Command.Equals(
                        commandName,
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    x.Command.Aliases.Any(a =>
                        a.Equals(
                            commandName,
                            StringComparison.OrdinalIgnoreCase)));
            }

            if (command == null)
            {
                Log.Warning($"Unknown command: {commandName}");
                return false;
            }

            try
            {
                return command.Command.Execute(args);
            }
            catch (Exception ex)
            {
                Log.Error(
                    $"Unhandled exception in command: {commandName}");

                Log.Error(ex);

                return false;
            }
        }

        public List<IConsoleCommand> GetCommands()
        {
            lock (sync)
            {
                return commands
                    .Select(x => x.Command)
                    .ToList();
            }
        }
    }
}