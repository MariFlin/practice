using System;
using System.Collections.Concurrent;
using System.Threading;

namespace task17;

public class ServerThread : IDisposable
{
    private readonly Thread _thread;
    private readonly BlockingCollection<ICommand> _queue = new();
    private CancellationTokenSource? _cts;
    private volatile bool _isRunning;

    public ServerThread()
    {
        _thread = new Thread(ProcessCommands)
        {
            IsBackground = true,
            Name = "ServerWorker"
        };
    }

    public void Start()
    {
        if (_isRunning) throw new InvalidOperationException("Поток уже запущен");
        _cts = new CancellationTokenSource();
        _isRunning = true;
        _thread.Start();
    }

    public void Enqueue(ICommand command)
    {
        if (!_isRunning) throw new InvalidOperationException("Поток не запущен или остановлен");
        _queue.Add(command);
    }

    private void ProcessCommands()
    {
        try
        {
            foreach (var command in _queue.GetConsumingEnumerable(_cts!.Token))
            {
                try
                {
                    command.Execute();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ExceptionHandler] Ошибка в команде {command.GetType().Name}: {ex.Message}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            
        }
        finally
        {
            _isRunning = false;
            _cts?.Dispose();
            _queue.Dispose();
        }
    }

    public void HardStop()
    {
        if (!IsCurrentWorkerThread())
            throw new InvalidOperationException("HardStop может быть вызван только внутри целевого потока");

        _cts?.Cancel();
    }

    public void SoftStop()
    {
        if (!IsCurrentWorkerThread())
            throw new InvalidOperationException("SoftStop может быть вызван только внутри целевого потока");

        _queue.CompleteAdding();
    }

    internal bool IsCurrentWorkerThread() => Thread.CurrentThread == _thread;

    public void Dispose()
    {
        if (_isRunning && _thread.IsAlive && Thread.CurrentThread != _thread)
        {
            _queue.CompleteAdding();
            _cts?.Cancel();
            _thread.Join(TimeSpan.FromSeconds(5));
        }
        _cts?.Dispose();
        _queue.Dispose();
    }
}
