using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ScottPlot;
using task14;

var sinFunc = (double x) => Math.Sin(x);
double a = -100, b = 100;
double targetPrecision = 1e-4;
int warmupRuns = 3;
int benchmarkRuns = 200;

Console.WriteLine($"Исследование: sin(x) на [{a}, {b}], точность {targetPrecision}\n");

var steps = new[] { 1e-1, 1e-2, 1e-3, 1e-4, 1e-5};
double optimalStep = 1e-5;
double bestTimeForStep = double.MaxValue;

Console.WriteLine("Поиск оптимального шага:");
foreach (var step in steps)
{
    for (int i = 0; i < warmupRuns; i++)
        DefiniteIntegral.Solve(a, b, sinFunc, step, 4);

    var sw = Stopwatch.StartNew();
    double result = 0;
    for (int i = 0; i < benchmarkRuns; i++)
        result = DefiniteIntegral.Solve(a, b, sinFunc, step, 4);
    sw.Stop();

    double avgMs = sw.ElapsedMilliseconds / (double)benchmarkRuns;
    Console.WriteLine($"  Шаг {step:E2} -> {avgMs:F2} мс, результат={result:F6}");

    if (Math.Abs(result) <= targetPrecision && avgMs < bestTimeForStep)
    {
        bestTimeForStep = avgMs;
        optimalStep = step;
    }
}
Console.WriteLine($"\nОптимальный шаг по точности: {optimalStep:E2}\n");
double benchmarkStep = optimalStep;
bool foundSuitableStep = false;

foreach (var step in steps.OrderBy(s => s))
{
    double testResult = DefiniteIntegral.Solve(a, b, sinFunc, step, 4);
    if (Math.Abs(testResult) > targetPrecision) continue;

    var sw = Stopwatch.StartNew();
    for (int i = 0; i < 10; i++)
        DefiniteIntegral.Solve(a, b, sinFunc, step, 4);
    sw.Stop();

    double avgMs = sw.ElapsedMilliseconds / 10.0;
    Console.WriteLine($"  Проверка {step:E2}: время={avgMs:F1} мс");

    if (avgMs >= 50.0)
    {
        benchmarkStep = step;
        foundSuitableStep = true;
        Console.WriteLine($"\nДля бенчмарка выбран шаг {benchmarkStep:E2} (~{avgMs:F0} мс)\n");
        break;
    }
}

if (!foundSuitableStep)
{
    benchmarkStep = steps
        .Where(s => Math.Abs(DefiniteIntegral.Solve(a, b, sinFunc, s, 4)) <= targetPrecision)
        .OrderBy(s => s).First();
    Console.WriteLine($"\nНи один шаг не дал >= 50 мс. Используем {benchmarkStep:E2}\n");
}

int maxThreads = Environment.ProcessorCount * 2;
var threadCounts = Enumerable.Range(1, maxThreads).ToArray();
var times = new List<double>();
var results = new List<double>();

Console.WriteLine("\nПоиск оптимального числа потоков:");
foreach (var threads in threadCounts)
{
    for (int i = 0; i < warmupRuns; i++)
        DefiniteIntegral.Solve(a, b, sinFunc, benchmarkStep, threads);

    var sw = Stopwatch.StartNew();
    double res = 0;
    for (int i = 0; i < benchmarkRuns; i++)
        res = DefiniteIntegral.Solve(a, b, sinFunc, benchmarkStep, threads);
    sw.Stop();

    double avgMs = sw.ElapsedMilliseconds / (double)benchmarkRuns;
    times.Add(avgMs);
    results.Add(res);
    Console.WriteLine($"  Потоки: {threads,3} -> {avgMs:F2} мс");
}

int minIdx = times.IndexOf(times.Min());
int optimalThreads = threadCounts[minIdx];
double optimalMultiTime = times[minIdx];
Console.WriteLine($"\nОптимальное число потоков: {optimalThreads} ({optimalMultiTime:F2} мс)");

var plt = new Plot();
plt.Add.Scatter(times.ToArray(), threadCounts.Select(t => (double)t).ToArray());
plt.Axes.Bottom.Label.Text = "Время вычисления Solve (мс)";
plt.Axes.Left.Label.Text = "Количество потоков";
plt.Axes.Title.Label.Text = $"Производительность ∫sin(x)dx на [{a},{b}], шаг={benchmarkStep:E2}";
plt.Grid.IsVisible = true;

var annotation = plt.Add.Annotation($"Оптимум: {optimalThreads} потоков\n{optimalMultiTime:F2} мс");
annotation.OffsetX = 20;
annotation.OffsetY = 20;

string chartPath = Path.Combine(AppContext.BaseDirectory, "performance_chart.png");
plt.SavePng(chartPath, 800, 600);
Console.WriteLine($"\nГрафик сохранен: {chartPath}");

static double SolveSingleThread(double a, double b, Func<double, double> f, double step)
{
    int n = (int)((b - a) / step);
    if (n == 0) return 0.0;
    
    double h = (b - a) / n;
    double sum = 0.5 * (f(a) + f(b));
    
    for (int i = 1; i < n; i++)
        sum += f(a + i * h);
    
    return sum * h;
}

for (int i = 0; i < warmupRuns; i++)
    SolveSingleThread(a, b, sinFunc, benchmarkStep);

var swSingle = Stopwatch.StartNew();
double singleResult = 0;
for (int i = 0; i < benchmarkRuns; i++)
    singleResult = SolveSingleThread(a, b, sinFunc, benchmarkStep);
swSingle.Stop();

double singleAvgMs = swSingle.ElapsedMilliseconds / (double)benchmarkRuns;
Console.WriteLine($"\nОднопоточная версия: {singleAvgMs:F2} мс");

double speedup = singleAvgMs > 0 ? singleAvgMs / optimalMultiTime : double.NaN;
double percentDiff = singleAvgMs > 0 ? ((singleAvgMs - optimalMultiTime) / singleAvgMs) * 100 : 0;

Console.WriteLine($"\nУскорение: {(double.IsNaN(speedup) ? "N/A" : $"{speedup:F2}x")} ({(double.IsNaN(percentDiff) ? "N/A" : $"+{percentDiff:F1}%")})");

if (percentDiff < 15)
    Console.WriteLine("️ Ускорение < 15%! Требуется оптимизация.");
else
    Console.WriteLine("Критерий выполнен: ускорение >= 15%");

string report = $"""
ОТЧЕТ ПО ЗАДАНИЮ 15
Дата: {DateTime.Now:dd.MM.yyyy HH:mm:ss}
Функция: sin(x)
Отрезок: [{a}, {b}]
Требуемая точность: {targetPrecision}

1. ОПТИМАЛЬНЫЙ РАЗМЕР ШАГА: {optimalStep:E2}
   Обеспечивает точность {targetPrecision} при минимальном времени выполнения.

2. ОПТИМАЛЬНОЕ КОЛИЧЕСТВО ПОТОКОВ: {optimalThreads}
   Достигнуто минимальное время выполнения функции Solve.

3. СРАВНЕНИЕ ПРОИЗВОДИТЕЛЬНОСТИ:
   - Время однопоточной версии: {singleAvgMs:F2} мс
   - Время многопоточной версии: {optimalMultiTime:F2} мс
   - Разница: {percentDiff:F1}% (ускорение в {speedup:F2} раза)
   - Результат многопоточной версии: {results[minIdx]:F6}
   - Результат однопоточной версии: {singleResult:F6}

4. ВЫВОД:
   {(percentDiff >= 15 
       ? $"Многопоточная реализация эффективнее однопоточной на {percentDiff:F1}%." 
       : "Требуется дополнительная оптимизация многопоточной версии.")}
""";

string reportPath = Path.Combine(AppContext.BaseDirectory, "optimization_report.txt");
File.WriteAllText(reportPath, report);
Console.WriteLine($"\nОтчет сохранен: {reportPath}");