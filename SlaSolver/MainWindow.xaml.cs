using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;
using SlaSolver.Solvers;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SlaSolver;

public partial class MainWindow : Window
{
    private const int MinSize = 2;
    private const int MaxSize = 6;
    private const int CurveResolution = 500;
    private const double TMin = -1.5;
    private const double TMax = 1.5;

    private static readonly SKColor[] Palette =
    {
        new(178, 34,  62),     // тёмно-красный (как на фото)
        new(30,  100, 200),    // синий
        new(34,  139, 62),     // зелёный
        new(220, 140, 20),     // оранжевый
        new(128, 50,  160),    // фиолетовый
        new(0,   150, 150),    // бирюзовый
        new(139, 90,  43)      // коричневый
    };

    private TextBox[,] _matrixBoxes = new TextBox[0, 0];
    private TextBox[] _bBoxes = Array.Empty<TextBox>();

    public MainWindow()
    {
        InitializeComponent();

        for (int i = MinSize; i <= MaxSize; i++)
            SizeCombo.Items.Add(i);

        foreach (var s in SolverFactory.All)
            MethodCombo.Items.Add(s);

        SizeCombo.SelectedIndex = 0;
        MethodCombo.SelectedIndex = 0;
    }

    private int CurrentSize => (int)SizeCombo.SelectedItem;

    private void SizeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (!IsLoaded && SizeCombo.SelectedItem == null) return;
        RebuildGrids();
    }

    private void RebuildGrids()
    {
        int n = CurrentSize;

        MatrixGrid.Columns = n;
        MatrixGrid.Children.Clear();
        _matrixBoxes = new TextBox[n, n];

        for (int r = 0; r < n; r++)
            for (int c = 0; c < n; c++)
            {
                var tb = MakeBox();
                _matrixBoxes[r, c] = tb;
                MatrixGrid.Children.Add(tb);
            }

        VectorBPanel.Children.Clear();
        _bBoxes = new TextBox[n];

        for (int r = 0; r < n; r++)
        {
            var tb = MakeBox();
            _bBoxes[r] = tb;
            VectorBPanel.Children.Add(tb);
        }

        SolutionChart.Series = null;
    }

    private static TextBox MakeBox() => new()
    {
        Width = 70,
        Height = 28,
        Margin = new Thickness(3),
        TextAlignment = TextAlignment.Center,
        VerticalContentAlignment = VerticalAlignment.Center,
        Text = "0",
        FontFamily = new FontFamily("Consolas")
    };

    private void SolveButton_Click(object sender, RoutedEventArgs e)
    {
        ResultBox.Clear();

        if (MethodCombo.SelectedItem is not ISolver solver)
        {
            ResultBox.Text = "Выберите метод.";
            return;
        }

        int n = CurrentSize;

        if (!TryReadInput(n, out var a, out var b, out var error))
        {
            ResultBox.Text = error;
            SolutionChart.Series = null;
            return;
        }

        try
        {
            if (CompareAllCheck.IsChecked == true)
            {
                var sb = new StringBuilder();
                foreach (var s in SolverFactory.All)
                {
                    try
                    {
                        var x = s.Solve(a, b);
                        sb.AppendLine($"── {s.MethodName} ──");
                        sb.AppendLine(FormatSolution(x));
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"── {s.MethodName} ── Ошибка: {ex.Message}");
                    }
                }
                ResultBox.Text = sb.ToString();
                DrawAllMethods(a, b);
            }
            else
            {
                var x = solver.Solve(a, b);
                ResultBox.Text = FormatSolution(x);
                DrawSingleMethod(x, solver.MethodName);
            }
        }
        catch (Exception ex)
        {
            ResultBox.Text = "Ошибка: " + ex.Message;
            SolutionChart.Series = null;
        }
    }

    private void DrawSingleMethod(double[] coeffs, string methodName)
    {
        var curve = ComputePolyCurve(coeffs);

        var series = new List<ISeries>
        {
            new LineSeries<ObservablePoint>
            {
                Values = curve,
                Name = methodName,
                Stroke = new SolidColorPaint(Palette[0]) { StrokeThickness = 3 },
                GeometrySize = 0,
                Fill = null,
                LineSmoothness = 1,
                Mapping = (p, i) => new(p.X ?? 0, p.Y ?? 0)
            },
            MakeZeroLineY(),
            MakeZeroLineX(curve)
        };

        SolutionChart.Series = series.ToArray();
        SetupAxes();
        SolutionChart.LegendPosition = LegendPosition.Hidden;
        SolutionChart.Title = new LabelVisual
        {
            Text = $"f(x) — {methodName}",
            TextSize = 14,
            Paint = new SolidColorPaint(SKColors.DarkSlateGray)
        };
    }

    private void DrawAllMethods(double[,] a, double[] b)
    {
        var all = SolverFactory.All.ToList();
        var series = new List<ISeries>();
        ObservablePoint[]? refCurve = null;

        for (int i = 0; i < all.Count; i++)
        {
            try
            {
                var coeffs = all[i].Solve(a, b);
                var curve = ComputePolyCurve(coeffs);
                refCurve ??= curve;

                series.Add(new LineSeries<ObservablePoint>
                {
                    Values = curve,
                    Name = all[i].MethodName,
                    Stroke = new SolidColorPaint(Palette[i % Palette.Length])
                    {
                        StrokeThickness = 2.5f
                    },
                    GeometrySize = 0,
                    Fill = null,
                    LineSmoothness = 1,
                    Mapping = (p, i) => new(p.X ?? 0, p.Y ?? 0)
                });
            }
            catch { }
        }

        if (refCurve != null)
        {
            series.Add(MakeZeroLineY());
            series.Add(MakeZeroLineX(refCurve));
        }

        SolutionChart.Series = series.ToArray();
        SetupAxes();
        SolutionChart.LegendPosition = LegendPosition.Right;
        SolutionChart.Title = new LabelVisual
        {
            Text = "Сравнение всех методов",
            TextSize = 14,
            Paint = new SolidColorPaint(SKColors.DarkSlateGray)
        };
    }

    private static ObservablePoint[] ComputePolyCurve(double[] c)
    {
        double step = (TMax - TMin) / CurveResolution;
        var pts = new ObservablePoint[CurveResolution + 1];

        for (int i = 0; i <= CurveResolution; i++)
        {
            double t = TMin + i * step;
            double y = Horner(c, t);
            pts[i] = new ObservablePoint(t, y);
        }
        return pts;
    }

    private static double Horner(double[] c, double t)
    {
        double r = c[c.Length - 1];
        for (int i = c.Length - 2; i >= 0; i--)
            r = r * t + c[i];
        return r;
    }

    private static LineSeries<ObservablePoint> MakeZeroLineY() => new()
    {
        Values = new[]
        {
            new ObservablePoint(TMin, 0),
            new ObservablePoint(TMax, 0)
        },
        Name = "y = 0",
        Stroke = new SolidColorPaint(new SKColor(0, 160, 220)) 
        { 
            StrokeThickness = 1.5f 
        },
        GeometrySize = 0,
        Fill = null,
        LineSmoothness = 0,
        Mapping = (p, i) => new(p.X ?? 0, p.Y ?? 0)
    };

    private static LineSeries<ObservablePoint> MakeZeroLineX(ObservablePoint[] curve)
    {
        double lo = curve.Min(p => p.Y ?? 0) - 0.5;
        double hi = curve.Max(p => p.Y ?? 0) + 0.5;

        return new LineSeries<ObservablePoint>
        {
            Values = new[]
            {
                new ObservablePoint(0, lo),
                new ObservablePoint(0, hi)
            },
            Name = "x = 0",
            Stroke = new SolidColorPaint(new SKColor(230, 190, 0)) { StrokeThickness = 1.5f },
            GeometrySize = 0,
            Fill = null,
            LineSmoothness = 0,
            Mapping = (p, i) => new(p.X ?? 0, p.Y ?? 0)
        };
    }

    private void SetupAxes()
    {
        SolutionChart.XAxes = new[]
        {
            new Axis
            {
                Name = "x",
                MinLimit = TMin,
                MaxLimit = TMax,
                SeparatorsPaint = new SolidColorPaint(new SKColor(230, 230, 230)),
                LabelsPaint = new SolidColorPaint(SKColors.Black),
                NamePaint = new SolidColorPaint(SKColors.DarkSlateGray),
                TextSize = 12
            }
        };

        SolutionChart.YAxes = new[]
        {
            new Axis
            {
                Name = "f(x)",
                SeparatorsPaint = new SolidColorPaint(new SKColor(230, 230, 230)),
                LabelsPaint = new SolidColorPaint(SKColors.Black),
                NamePaint = new SolidColorPaint(SKColors.DarkSlateGray),
                TextSize = 12
            }
        };
    }
    private bool TryReadInput(int n, out double[,] a, out double[] b, out string error)
    {
        a = new double[n, n];
        b = new double[n];
        error = "";

        for (int r = 0; r < n; r++)
        {
            for (int c = 0; c < n; c++)
            {
                if (!TryParse(_matrixBoxes[r, c].Text, out a[r, c]))
                {
                    error = $"Некорректное число в A[{r + 1},{c + 1}].";
                    return false;
                }
            }
            if (!TryParse(_bBoxes[r].Text, out b[r]))
            {
                error = $"Некорректное число в b[{r + 1}].";
                return false;
            }
        }
        return true;
    }

    private static bool TryParse(string text, out double value) =>
        double.TryParse(text.Replace(',', '.'), NumberStyles.Float,
                        CultureInfo.InvariantCulture, out value);

    private static string FormatSolution(double[] x)
    {
        var sb = new StringBuilder();
        for (int i = 0; i < x.Length; i++)
            sb.AppendLine($"x{i + 1} = {x[i]:0.######}");
        return sb.ToString();
    }
}
