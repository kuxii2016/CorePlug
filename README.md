# CorePlug

Lightweight, extensible, and plugin-driven game server framework written in C#.

Proxyserver provides a modular runtime environment designed for multiplayer games, backend services, and extensible server applications.  
The framework focuses on stability, simplicity, performance, and clean plugin integration.

---

# Features

## Plugin System
- Dynamic `.dll` plugin loading
- Isolated `AssemblyLoadContext`
- Plugin lifecycle management
- Runtime plugin enable/disable
- Version compatibility checks
- Safe plugin unloading
- Thread-safe execution

## Event System
- Lightweight event bus
- Thread-safe subscriptions
- Plugin-to-host communication
- Automatic event cleanup
- Exception-safe event execution

## Console Command System
- Runtime command registration
- Plugin-specific commands
- Command aliases
- Usage descriptions
- Autocomplete support
- Thread-safe command execution

## Logging System
- Colored console logging
- Debug logging support
- Verbose/full logging mode
- File logging
- Caller tracing
- Plugin-prefixed log messages
- Log rotation support

## Configuration System
- JSON-based configuration files
- Strongly typed configs
- Automatic config generation
- Plugin-specific config directories

---

# Architecture

Host Application

- PluginManager
  - Plugin Loading
  - Lifecycle Management
  - Tick Execution
  - Plugin Isolation

- EventBus
  - Subscribe<T>()
  - Publish<T>()

- ConsoleCommandManager
  - RegisterCommand()
  - Execute()

- Plugins
  - Commands
  - Events
  - Configurations
  - Runtime Logic

---

# Plugin Lifecycle

Load → Start → Tick → Stop → Dispose → Unload

---

# Example Plugin

```csharp
using Plugin;
using System;
using System.Threading;

public class ExamplePlugin : IPlugin
{
    public string Name => "Example Plugin";
    public string Author => "Developer";
    public string Description => "Simple example plugin";

    public Version Version => new Version(1, 0, 0);
    public Version MinimumHostVersion => new Version(1, 0, 0);

    public void Start(IPluginContext context, CancellationToken token)
    {
        context.Logger.Info("Plugin started");

        context.Events.Subscribe<PlayerJoinEvent>(
            Name,
            OnPlayerJoin);
    }

    private void OnPlayerJoin(PlayerJoinEvent ev)
    {
        Console.WriteLine($"Player joined: {ev.PlayerName}");
    }

    public void OnTick(IPluginContext context) { }

    public void Stop(IPluginContext context)
    {
        context.Logger.Info("Plugin stopped");
    }

    public void Dispose() { }
}
```

---

# Event Example

Subscribe:
context.Events.Subscribe<PlayerJoinEvent>(Name, OnPlayerJoin);

Publish:
PluginManager.Events.Publish(new PlayerJoinEvent { PlayerName = "Player" });

---

# Built-in Commands

- help → Displays all available commands  
- plugins → Lists all loaded plugins  
- pluginstatus → Shows plugin status  
- startplugin → Starts a plugin  
- stopplugin → Stops a plugin  
- debug → Toggles debug logging  
- fulllog → Toggles verbose logging  
- uptime → Shows server uptime  

---

# Goals

- Lightweight architecture
- Easy plugin development
- Stable runtime execution
- High extensibility
- Minimal dependencies
- Clean API design

---

# Planned Features

- Async event dispatching
- Plugin dependency system
- Hot reload
- Scheduler API
- Remote admin API
- Web dashboard
- Permission system
- Plugin marketplace

---

# License

MIT License
