using System;
using System.Collections.Concurrent;
using System.Threading;
using task17;

namespace task18;

public class ScheduledServerThread : IDisposable
{
    private readonly Thread _thread;
    private readonly BlockingCollection<ICommand> _newCommands = new();
    private readonly IScheduler _scheduler;
    private readonly ManualResetEventSlim _workAvailable = new(false);
    private volatile bool _isRunning;

    public ScheduledServerThread(IScheduler scheduler)
    {
        _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
        _thread = new Thread(ProcessCommands)
        {
            IsBackground = true,
            Name = "ScheduledWorker"
        };
    }

    public void Start()
    {
        if (_isRunning) throw new InvalidOperationException("Поток уже запущен");
        _isRunning = true;
        _thread.Start();
    }

    public void EnqueueNew(ICommand command)
    {
        if (!_isRunning) throw new InvalidOperationException("Поток остановлен");
        _newCommands.Add(command);
        _workAvailable.Set();
    }

    public void Reschedule(ICommand command)
    {
        if (!_isRunning) throw new InvalidOperationException("Поток остановлен");
        _scheduler.Add(command);
        _workAvailable.Set();
    }

private void ProcessCommands()
{
    try
    {
        while (_isRunning)
        {
            ICommand? nextCommand = null;

            if (_scheduler.HasCommand())
            {
                nextCommand = _scheduler.Select();
            }
            else if (_newCommands.TryTake(out var newCmd, TimeSpan.Zero))
            {
                nextCommand = newCmd;
            }
            else
            {
                _workAvailable.Wait(TimeSpan.FromMilliseconds(50));
                _workAvailable.Reset();
                
                if (_scheduler.HasCommand())
                    nextCommand = _scheduler.Select();
                else if (_newCommands.TryTake(out newCmd, TimeSpan.Zero))
                    nextCommand = newCmd;
            }

            if (nextCommand != null)
            {
                try
                {
                    nextCommand.Execute();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ExceptionHandler] Ошибка в команде {nextCommand.GetType().Name}: {ex.Message}");
                }
            }
        }
    }
    finally
    {
        _isRunning = false;
        _newCommands.Dispose();
        _workAvailable.Dispose();
    }
}

    public void Stop()
    {
        _isRunning = false;
        
        _workAvailable.Set(); 
        
    }

    public void Dispose()
    {
        if (_thread.IsAlive)
        {
            Stop();
            _thread.Join(TimeSpan.FromSeconds(5));
        }
        
        try 
        { 
            _newCommands.CompleteAdding(); 
        } 
        catch {  }
        
        _newCommands.Dispose();
        _workAvailable.Dispose();
    }
}