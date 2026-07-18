using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using task17;
using task18;

namespace task18;

public static class Benchmarker
{
    public static (double[] xLabels, double[] withoutScheduler, double[] withScheduler) Run()
    {
        int[] threadCounts = { 1, 2, 4, 8 };
        var withoutScheduler = new double[threadCounts.Length];
        var withScheduler = new double[threadCounts.Length];

        for (int i = 0; i < threadCounts.Length; i++)
        {
            int threads = threadCounts[i];
            
            withoutScheduler[i] = MeasureThroughput(threads, useScheduler: false);
            
            withScheduler[i] = MeasureThroughput(threads, useScheduler: true);
        }

        return (threadCounts.Select(t => (double)t).ToArray(), withoutScheduler, withScheduler);
    }

    private static double MeasureThroughput(int workerCount, bool useScheduler)
    {
        const int totalOperations = 1_000;
        const int longOpDurationMs = 2;
        
        var completedOps = 0;
        
        if (useScheduler)
        {
            var scheduler = new RoundRobinScheduler();
            var server = new ScheduledServerThread(scheduler);
            server.Start();

            for (int i = 0; i < totalOperations; i++)
                scheduler.Add(new LongTask(() => Interlocked.Increment(ref completedOps), longOpDurationMs));

            var sw = Stopwatch.StartNew();
            
            bool success = SpinWait.SpinUntil(() => completedOps >= totalOperations, TimeSpan.FromSeconds(120));
            sw.Stop();
            
            if (!success) throw new TimeoutException("Бенчмарк с планировщиком не уложился в таймаут");

            server.Stop();
            return totalOperations / (sw.ElapsedMilliseconds / 1000.0);
        }
        else
        {
            var queue = new BlockingCollection<ICommand>();
            var workers = new Thread[workerCount];
            var cts = new CancellationTokenSource();

            for (int w = 0; w < workerCount; w++)
            {
                workers[w] = new Thread(() =>
                {
                    try 
                    {
                        foreach (var cmd in queue.GetConsumingEnumerable(cts.Token))
                            cmd.Execute();
                    }
                    catch (OperationCanceledException) 
                    {
                    }
                }) { IsBackground = true };
                workers[w].Start();
            }

            for (int i = 0; i < totalOperations; i++)
                queue.Add(new LongTask(() => Interlocked.Increment(ref completedOps), longOpDurationMs));
            
            queue.CompleteAdding();

            var sw = Stopwatch.StartNew();
            
            bool success = SpinWait.SpinUntil(() => completedOps >= totalOperations, TimeSpan.FromSeconds(120));
            sw.Stop();

            if (!success) throw new TimeoutException("Бенчмарк без планировщика не уложился в таймаут");

            cts.Cancel();
            foreach (var t in workers) t.Join(TimeSpan.FromSeconds(5));
            
            cts.Dispose();
            return totalOperations / (sw.ElapsedMilliseconds / 1000.0);
        }
    }

    private class LongTask : ICommand
    {
        private readonly Action _onComplete;
        private readonly int _delayMs;
        public LongTask(Action onComplete, int delayMs) 
        { 
            _onComplete = onComplete; 
            _delayMs = delayMs; 
        }
        public void Execute() 
        { 
            Thread.Sleep(_delayMs); 
            _onComplete(); 
        }
    }
}