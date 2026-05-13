using System;
using System.Collections.Generic;
using Utils.Logging;

namespace Plugin
{
    /// <summary>
    /// Thread-safe event bus implementation used for communication
    /// between plugins and the host application.
    /// </summary>
    public class EventBus : IEventBus
    {
        /// <summary>
        /// Represents a single event subscription entry.
        /// </summary>
        private class Subscription
        {
            /// <summary>
            /// Name of the plugin that registered the subscription.
            /// </summary>
            public string PluginName;

            /// <summary>
            /// Delegate callback invoked when the event is published.
            /// </summary>
            public Delegate Callback;
        }

        private readonly Dictionary<Type, List<Subscription>> handlers
            = new Dictionary<Type, List<Subscription>>();

        private readonly object sync = new object();

        /// <summary>
        /// Subscribes a plugin to a specific event type.
        /// </summary>
        /// <typeparam name="T">Type of the event.</typeparam>
        /// <param name="pluginName">Name of the subscribing plugin.</param>
        /// <param name="callback">Callback executed when event is published.</param>
        public void Subscribe<T>(string pluginName, Action<T> callback)
        {
            Type type = typeof(T);

            lock (sync)
            {
                if (!handlers.TryGetValue(type, out List<Subscription> list))
                {
                    list = new List<Subscription>();
                    handlers[type] = list;
                }

                list.Add(new Subscription
                {
                    PluginName = pluginName,
                    Callback = callback
                });
            }

            Log.Debug(
                $"Event subscription added: Plugin='{pluginName}', Event='{type.Name}'");
        }

        /// <summary>
        /// Publishes an event to all subscribed handlers of the event type.
        /// </summary>
        /// <typeparam name="T">Type of the event.</typeparam>
        /// <param name="ev">Event instance to publish.</param>
        public void Publish<T>(T ev)
        {
            Type type = typeof(T);

            List<Subscription> executionList;

            lock (sync)
            {
                if (!handlers.TryGetValue(type, out List<Subscription> list))
                    return;

                executionList = new List<Subscription>(list);
            }

            foreach (Subscription subscription in executionList)
            {
                try
                {
                    ((Action<T>)subscription.Callback)(ev);
                }
                catch (Exception ex)
                {
                    Log.Error(
                        $"Unhandled exception in event handler: " +
                        $"Plugin='{subscription.PluginName}', Event='{type.Name}'");

                    Log.Error(ex);
                }
            }
        }

        /// <summary>
        /// Removes all event subscriptions belonging to a specific plugin.
        /// Typically used when a plugin is stopped or unloaded.
        /// </summary>
        /// <param name="pluginName">Name of the plugin to remove subscriptions for.</param>
        public void RemovePluginSubscriptions(string pluginName)
        {
            lock (sync)
            {
                foreach (var pair in handlers)
                {
                    pair.Value.RemoveAll(x => x.PluginName == pluginName);
                }
            }

            Log.Debug(
                $"Removed all event subscriptions for plugin: {pluginName}");
        }
    }
}