using System.IO;
using task18;
using ScottPlot;

var (xLabels, without, with) = Benchmarker.Run();

var plt = new Plot();
var barWithout = plt.Add.Bars(without);
barWithout.LegendText = "Без планировщика (ops/sec)";

var barWith = plt.Add.Bars(with);
barWith.LegendText = "С Round Robin (ops/sec)";

double[] positions = Enumerable.Range(0, xLabels.Length).Select(i => (double)i).ToArray();
string[] labels = xLabels.Select(v => v.ToString()).ToArray();
plt.Axes.Bottom.SetTicks(positions, labels);
plt.Axes.Left.Label.Text = "Операций в секунду";
plt.Axes.Bottom.Label.Text = "Количество рабочих потоков";
plt.Title("Эффективность планировщика длительных операций");
plt.Legend.IsVisible = true;
plt.Grid.IsVisible = true;

string path = Path.Combine(Directory.GetCurrentDirectory(), "scheduler_performance.png");
plt.SavePng(path, 900, 600);

Console.WriteLine($"Без планировщика: {without.Average():F0} ops/sec");
Console.WriteLine($"С планировщиком:   {with.Average():F0} ops/sec");

if (!File.Exists(path))
    throw new FileNotFoundException("График не был сохранен!", path);

var fileInfo = new FileInfo(path);
if (fileInfo.Length < 1024)
    throw new Exception($"Файл графика слишком мал ({fileInfo.Length} байт). Возможно, данные были пустыми.");

Console.WriteLine($"График сохранен: {path} ({fileInfo.Length:N0} байт)");