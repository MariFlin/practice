using System;
using System.Threading;

namespace task14;

public static class DefiniteIntegral
{
    public static double Solve(double a, double b, Func<double, double> function, double step, int threadsNumber)
    {
        if (threadsNumber <= 0) throw new ArgumentOutOfRangeException(nameof(threadsNumber));
        if (step <= 0) throw new ArgumentOutOfRangeException(nameof(step));
        if (Math.Abs(b - a) < 1e-15) return 0.0;

        double totalSum = 0.0;

        var barrier = new Barrier(threadsNumber + 1);

        double segmentSize = (b - a) / threadsNumber;

        Thread[] threads = new Thread[threadsNumber];
        for (int i = 0; i < threadsNumber; i++)
        {
            double localA = a + i * segmentSize;
            double localB = a + (i + 1) * segmentSize;

            threads[i] = new Thread(() =>
            {
                double localSum = TrapezoidRule(localA, localB, function, step);

                AddToTotal(ref totalSum, localSum);

                barrier.SignalAndWait();
            });

            threads[i].Start();
        }

        barrier.SignalAndWait();

        return totalSum;
    }

    private static double TrapezoidRule(double localA, double localB, Func<double, double> f, double step)
    {
        int n = (int)((localB - localA) / step);
        if (n == 0) return 0.0;

        double h = (localB - localA) / n;
        double sum = 0.5 * (f(localA) + f(localB));

        for (int i = 1; i < n; i++)
        {
            sum += f(localA + i * h);
        }

        return sum * h;
    }

    private static void AddToTotal(ref double total, double value)
    {
        double current, newValue;
        do
        {
            current = total;
            newValue = current + value;
        } while (Interlocked.CompareExchange(ref total, newValue, current) != current);
    }
}