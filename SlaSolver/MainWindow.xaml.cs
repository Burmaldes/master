using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SlaSolver.Solvers;

namespace SlaSolver;

public partial class MainWindow : Window
{
    private const int MinSize = 2;
    private const int MaxSize = 6;

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
            return;
        }

        try
        {
            var x = solver.Solve(a, b);
            ResultBox.Text = FormatSolution(x);
        }
        catch (Exception ex)
        {
            ResultBox.Text = "Ошибка: " + ex.Message;
        }
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
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < x.Length; i++)
            sb.AppendLine($"x{i + 1} = {x[i]:0.######}");
        return sb.ToString();
    }
}