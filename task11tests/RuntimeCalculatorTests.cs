using Xunit;
using task11;

namespace task11tests;

public class RuntimeCalculatorTests
{
    private const string CalculatorSource = @"
public class Calculator : task11.ICalculator
{
    public int Add(int a, int b) => a + b;
    public int Minus(int a, int b) => a - b;
    public int Mul(int a, int b) => a * b;
    public int Div(int a, int b) => a / b;
}";

    [Fact]
    public void Add_ShouldReturnCorrectSum()
    {
        var calc = RuntimeClassGenerator.CreateCalculator(CalculatorSource);
        Assert.Equal(7, calc.Add(3, 4));
    }

    [Fact]
    public void Minus_ShouldReturnCorrectDifference()
    {
        var calc = RuntimeClassGenerator.CreateCalculator(CalculatorSource);
        Assert.Equal(2, calc.Minus(5, 3));
    }

    [Fact]
    public void Mul_ShouldReturnCorrectProduct()
    {
        var calc = RuntimeClassGenerator.CreateCalculator(CalculatorSource);
        Assert.Equal(12, calc.Mul(3, 4));
    }

    [Fact]
    public void Div_ShouldReturnCorrectQuotient()
    {
        var calc = RuntimeClassGenerator.CreateCalculator(CalculatorSource);
        Assert.Equal(3, calc.Div(10, 3)); // Целочисленное деление
    }

    [Fact]
    public void InvalidSourceCode_ShouldThrowInvalidOperationException()
    {
        var invalidSource = "public class Broken { syntax error!!! }";
        Assert.Throws<System.InvalidOperationException>(() => 
            RuntimeClassGenerator.CreateCalculator(invalidSource));
    }

    [Fact]
    public void GeneratedInstance_ShouldImplementICalculator()
    {
        var calc = RuntimeClassGenerator.CreateCalculator(CalculatorSource);
        Assert.IsAssignableFrom<ICalculator>(calc);
    }
}