using Xunit;
using System;
using System.Diagnostics;
using task14;

namespace task15tests;

public class DefiniteIntegralTests
{
    private static readonly Func<double, double> SinFunc = Math.Sin;
    private static readonly Func<double, double> LinearFunc = x => x;


    [Fact]
    public void Solve_SinSymmetricInterval_ReturnsZero()
    {
        // ∫[-100,100] sin(x) dx = 0
        double result = DefiniteIntegral.Solve(-100, 100, SinFunc, 1e-4, 4);
        Assert.Equal(0, result, 1e-3);
    }

    [Fact]
    public void Solve_LinearFunction_KnownResult()
    {
        // ∫[0,10] x dx = 50
        double result = DefiniteIntegral.Solve(0, 10, LinearFunc, 1e-5, 4);
        Assert.Equal(50, result, 1e-3);
    }

    [Fact]
    public void Solve_SingleThread_EqualsMultiThread()
    {
        double single = DefiniteIntegral.Solve(0, 50, SinFunc, 1e-4, 1);
        double multi = DefiniteIntegral.Solve(0, 50, SinFunc, 1e-4, 8);
        Assert.Equal(single, multi, 1e-3);
    }


    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Solve_InvalidThreadsNumber_ThrowsArgumentOutOfRange(int threads)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DefiniteIntegral.Solve(0, 1, SinFunc, 1e-4, threads));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.001)]
    public void Solve_InvalidStep_ThrowsArgumentOutOfRange(double step)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            DefiniteIntegral.Solve(0, 1, SinFunc, step, 4));
    }

    [Fact]
    public void Solve_ZeroLengthInterval_ReturnsZero()
    {
        double result = DefiniteIntegral.Solve(5, 5, SinFunc, 1e-4, 4);
        Assert.Equal(0, result, 1e-10);
    }

    [Fact]
    public void Solve_MultiThreadFasterThanSingle_OnHeavyLoad()
    {
        const double a = -100, b = 100;
        const double step = 1e-5;
        const int warmup = 3;
        const int runs = 50;

        for (int i = 0; i < warmup; i++)
            DefiniteIntegral.Solve(a, b, SinFunc, step, 1);
        for (int i = 0; i < warmup; i++)
            DefiniteIntegral.Solve(a, b, SinFunc, step, 4);

        var swSingle = Stopwatch.StartNew();
        for (int i = 0; i < runs; i++)
            DefiniteIntegral.Solve(a, b, SinFunc, step, 1);
        swSingle.Stop();
        double singleMs = swSingle.ElapsedMilliseconds / (double)runs;

        var swMulti = Stopwatch.StartNew();
        for (int i = 0; i < runs; i++)
            DefiniteIntegral.Solve(a, b, SinFunc, step, 4);
        swMulti.Stop();
        double multiMs = swMulti.ElapsedMilliseconds / (double)runs;

        double speedupPercent = ((singleMs - multiMs) / singleMs) * 100;
        
        Assert.True(speedupPercent >= 15,
            $"Многопоточная версия должна быть быстрее минимум на 15%. " +
            $"Однопоточная: {singleMs:F1} мс, Многопоточная: {multiMs:F1} мс, " +
            $"Ускорение: {speedupPercent:F1}%");
    }
}