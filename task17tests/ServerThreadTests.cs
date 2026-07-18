using Xunit;
using System;
using System.Collections.Generic;
using System.Threading;
using task17;

namespace task17tests;

public class ServerThreadTests
{
    [Fact]
    public void SoftStop_ProcessesAllCommandsBeforeStopping()
    {
        var executedCommands = new List<string>();
        var server = new ServerThread();
        server.Start();

        for (int i = 0; i < 3; i++)
        {
            int index = i;
            server.Enqueue(new ActionCommand(() => executedCommands.Add($"cmd_{index}")));
        }
        server.Enqueue(new SoftStopCommand(server));

        Thread.Sleep(500);

        Assert.Equal(new[] { "cmd_0", "cmd_1", "cmd_2" }, executedCommands);
        server.Dispose();
    }

    [Fact]
    public void HardStop_FromExternalThread_ThrowsException()
    {
        var server = new ServerThread();
        server.Start();

        var cmd = new HardStopCommand(server);
        Assert.Throws<InvalidOperationException>(() => cmd.Execute());

        server.Dispose();
    }

    [Fact]
    public void SoftStop_FromExternalThread_ThrowsException()
    {
        var server = new ServerThread();
        server.Start();

        var cmd = new SoftStopCommand(server);
        Assert.Throws<InvalidOperationException>(() => cmd.Execute());

        server.Dispose();
    }

    [Fact]
    public void EmptyQueue_DoesNotConsumeCpu()
    {
        var server = new ServerThread();
        server.Start();

        var sw = System.Diagnostics.Stopwatch.StartNew();
        Thread.Sleep(200);
        sw.Stop();

        Assert.True(sw.ElapsedMilliseconds >= 190);
        server.Dispose();
    }

    private class ActionCommand : ICommand
    {
        private readonly Action _action;
        public ActionCommand(Action action) => _action = action;
        public void Execute() => _action();
    }
}