using System;

namespace Plugin
{
    /// <summary>
    /// Provides a lightweight event bus for communication between plugins
    /// and the host application.
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// Subscribes a plugin to a specific event type.
        /// </summary>
        /// <typeparam name="T">The type of event to subscribe to.</typeparam>
        /// <param name="pluginName">The name of the plugin registering the subscription.</param>
        /// <param name="callback">The callback invoked when the event is published.</param>
        void Subscribe<T>(string pluginName, Action<T> callback);

        /// <summary>
        /// Publishes an event to all subscribed handlers of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of event being published.</typeparam>
        /// <param name="ev">The event instance to publish.</param>
        void Publish<T>(T ev);
    }
}