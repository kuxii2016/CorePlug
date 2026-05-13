namespace Utils.ServerConsole
{
    public class RegisteredCommand
    {
        public string Registar { get; set; }
        public string PluginName { get; set; }

        public IConsoleCommand Command { get; set; }
    }
}
