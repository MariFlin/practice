using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using task17;
using task18;

namespace task18tests;

public class ScheduledServerTests
{
    [Fact]
    public void RoundRobin_AlternatesBetweenLongRunningCommands()
    {
        var executionOrder = new List<string>();
        var scheduler = new RoundRobinScheduler();
        var server = new ScheduledServerThread(scheduler);
        
        var barrier = new Barrier(2); 

        server.Start();

        scheduler.Add(new LongRunningCommand("A", 2, executionOrder, scheduler));
        scheduler.Add(new LongRunningCommand("B", 2, executionOrder, scheduler));
        
        bool success = SpinWait.SpinUntil(() => executionOrder.Count >= 4, TimeSpan.FromSeconds(5));
        
        server.Dispose();

        Assert.True(success, $"Ожидалось 4 операции, выполнено: {executionOrder.Count}");
        Assert.Equal(new[] { "A", "B", "A", "B" }, executionOrder);
    }

    [Fact]
    public void NewCommands_ProcessedWhenSchedulerIsEmpty()
    {
        var executed = new List<string>();
        var scheduler = new RoundRobinScheduler();
        var server = new ScheduledServerThread(scheduler);
        server.Start();

        server.EnqueueNew(new ActionCommand(() => executed.Add("new1")));
        server.EnqueueNew(new ActionCommand(() => executed.Add("new2")));

        Thread.Sleep(500);
        server.Stop();

        Assert.Equal(new[] { "new1", "new2" }, executed);
    }

    [Fact]
    public void NoDeadlock_WhenReschedulingFromExecute()
    {
        var completed = false;
        var scheduler = new RoundRobinScheduler();
        var server = new ScheduledServerThread(scheduler);
        server.Start();

        scheduler.Add(new SelfReschedulingCommand(server, 3, () => completed = true));

        SpinWait.SpinUntil(() => completed, TimeSpan.FromSeconds(5));
        server.Stop();

        Assert.True(completed, "Команда должна была завершиться без deadlock");
    }

    [Fact]
    public void EmptyScheduler_DoesNotCauseBusyWait()
    {
        var scheduler = new RoundRobinScheduler();
        var server = new ScheduledServerThread(scheduler);
        server.Start();

        var sw = System.Diagnostics.Stopwatch.StartNew();
        Thread.Sleep(300);
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds >= 290, "Поток должен блокироваться при пустом планировщике");
        server.Stop();
    }


    private class LongRunningCommand : ICommand
    {
        private readonly string _name;
        private int _remainingCalls;
        private readonly List<string> _log;
        private readonly IScheduler _scheduler;

        public LongRunningCommand(string name, int calls, List<string> log, IScheduler scheduler)
        {
            _name = name;
            _remainingCalls = calls;
            _log = log;
            _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
        }

        public void Execute()
        {
            Thread.Sleep(1);
            lock (_log) _log.Add(_name);
            _remainingCalls--;
            
            if (_remainingCalls > 0)
                _scheduler.Add(this);
        }
    }

    private class SelfReschedulingCommand : ICommand
    {
        private readonly ScheduledServerThread _server;
        private int _remainingCalls;
        private readonly Action _onComplete;

        public SelfReschedulingCommand(ScheduledServerThread server, int calls, Action onComplete)
        {
            _server = server ?? throw new ArgumentNullException(nameof(server));
            _remainingCalls = calls;
            _onComplete = onComplete ?? throw new ArgumentNullException(nameof(onComplete));
        }

        public void Execute()
        {
            _remainingCalls--;
            if (_remainingCalls > 0)
                _server.Reschedule(this);
            else
                _onComplete();
        }
    }

    private class ActionCommand : ICommand
    {
        private readonly Action _action;
        public ActionCommand(Action action) => _action = action ?? throw new ArgumentNullException(nameof(action));
        public void Execute() => _action();
    }
}