using System;
using System.Threading;
using Utils.ConsoleCommands;
using Utils.Logging;

namespace Utils.ServerConsole
{
    public class InputConsole
    {
        private readonly ConsoleCommandManager commandManager;

        private Thread consoleThread;

        private bool running;

        public InputConsole(ConsoleCommandManager commandManager)
        {
            this.commandManager = commandManager;
        }

        public void CreateConsole()
        {
            if (running)
                return;
            commandManager.RegisterCommand("Host", new UptimeCommand());

            commandManager.RegisterCommand("Host", new PrettyLogCommand());
            commandManager.RegisterCommand("Host", new DebugCommand());
            commandManager.RegisterCommand("Host", new Fulllog());
            commandManager.RegisterCommand("Host", new HelpCommand(commandManager));

            running = true;

            consoleThread = new Thread(ConsoleLoop);
            consoleThread.Name = "Console Thread";
            consoleThread.Start();
            Log.Info("Console thread started");
        }

        public void Stop()
        {
            running = false;
        }

        private void ConsoleLoop()
        {
            Console.CursorVisible = true;

            Log.Info("Console initialized");
            Log.Info("Type 'help' for a list of commands");

            while (running)
            {
                try
                {
                    string input = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(input))
                        continue;

                    if (input.Equals(
                        "exit",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        Log.Info("Shutdown requested from console");

                        running = false;

                        break;
                    }
                    commandManager.Execute(input);
                }
                catch (Exception ex)
                {
                    Log.Error("Console processing failed");
                    Log.Error(ex);
                }

                Thread.Sleep(1);
            }

            Console.CursorVisible = false;

            Log.Info("Console thread stopped");
        }
    }
}