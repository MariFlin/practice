using System;
using System.Collections.Generic;
using task17;

namespace task18;

public class RoundRobinScheduler : IScheduler
{
    private readonly Queue<ICommand> _queue = new();
    private readonly object _lock = new();

    public bool HasCommand()
    {
        lock (_lock)
            return _queue.Count > 0;
    }

    public ICommand Select()
    {
        lock (_lock)
        {
            if (_queue.Count == 0)
                throw new InvalidOperationException("Планировщик пуст");

            var command = _queue.Dequeue();
            _queue.Enqueue(command);
            return command;
        }
    }

    public void Add(ICommand cmd)
    {
        if (cmd == null) throw new ArgumentNullException(nameof(cmd));
        
        lock (_lock)
            _queue.Enqueue(cmd);
    }
}