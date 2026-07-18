using Xunit;
using task14;

namespace task14tests;

public class DefiniteIntegralTests
{
    private static readonly Func<double, double> X = x => x;
    private static readonly Func<double, double> SIN = Math.Sin;

    [Fact]
    public void Solve_LinearFunction_SymmetricInterval_ReturnsZero()
    {
        // ∫[-1,1] x dx = 0
        double result = DefiniteIntegral.Solve(-1, 1, X, 1e-4, 2);
        Assert.Equal(0d, result, 1e-4);
    }

    [Fact]
    public void Solve_SinFunction_SymmetricInterval_ReturnsZero()
    {
        // ∫[-1,1] sin(x) dx = 0
        double result = DefiniteIntegral.Solve(-1, 1, SIN, 1e-5, 8);
        Assert.Equal(0d, result, 1e-4);
    }

    [Fact]
    public void Solve_LinearFunction_PositiveInterval_ReturnsCorrectArea()
    {
        double result = DefiniteIntegral.Solve(0, 5, X, 1e-6, 8);
        
        Assert.Equal(12.5d, result, 1e-5);
    }

    [Fact]
    public void Solve_SingleThread_ReturnsCorrectResult()
    {
        double result = DefiniteIntegral.Solve(0, 1, X, 1e-6, 1);
        Assert.Equal(0.5d, result, 1e-5);
    }

    [Fact]
    public void Solve_ManyThreads_ReturnsConsistentResult()
    {
        double r1 = DefiniteIntegral.Solve(0, 10, SIN, 1e-5, 1);
        double r2 = DefiniteIntegral.Solve(0, 10, SIN, 1e-5, 16);
        
        Assert.Equal(r1, r2, 1e-4);
    }

    [Fact]
    public void Solve_InvalidThreadsNumber_ThrowsException()
    {
        Assert.Throws<System.ArgumentOutOfRangeException>(() =>
            DefiniteIntegral.Solve(0, 1, X, 1e-4, 0));
    }

    [Fact]
    public void Solve_ZeroStep_ThrowsException()
    {
        Assert.Throws<System.ArgumentOutOfRangeException>(() =>
            DefiniteIntegral.Solve(0, 1, X, 0, 2));
    }
}