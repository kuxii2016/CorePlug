using System.Collections.Generic;

namespace Utils.ServerConsole
{
    public interface IConsoleCommand
    {
        string Command { get; }

        string Description { get; }

        string Usage { get; }

        IEnumerable<string> Aliases { get; }

        bool Execute(string[] args);
    }
}
