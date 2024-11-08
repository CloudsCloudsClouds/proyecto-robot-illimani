using System;
using System.Collections.Generic;
using Godot;

public class EventBus
{
    private readonly Dictionary<Type, List<Action<object>>> _subscribers = new();
    public bool DebugEventBus = false;


    public void Subscribe<TEvent>(Action<TEvent> handler) where TEvent : class
    {
        var eventType = typeof(TEvent);
        if (!_subscribers.ContainsKey(eventType))
        {
            _subscribers[eventType] = new List<Action<object>>();
        }
        _subscribers[eventType].Add(e => handler((TEvent)e));
    }

    public void Unsubscribe<TEvent>(Action<TEvent> handler) where TEvent : class
    {
        var eventType = typeof(TEvent);
        if (_subscribers.ContainsKey(eventType))
        {
            _subscribers[eventType].RemoveAll(h => h.Method == handler.Method && h.Target == handler.Target);
        }
    }

    public void Publish<TEvent>(TEvent eventData) where TEvent : class
    {
        var eventType = typeof(TEvent);
        if (_subscribers.TryGetValue(eventType, out var handlers))
        {
            foreach (var handler in handlers.ToArray()) // Create a copy to avoid modification during iteration
            {
                try
                {
                    handler?.Invoke(eventData);
                }
                catch (Exception ex)
                {
                    GD.PrintErr($"EventBus: Error handling event {eventType.Name}: {ex.Message}");
                }
            }
        }
    }

    public void Ping()
    {
        GD.Print("ping");
    }
}
