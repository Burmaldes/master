using SlaSolver.Solvers;
using System;

public class GaussJordanSolver : ISolver
{
    public string MethodName => "Метод Гаусса-Жордана";

    public double[] Solve(double[,] matrixA, double[] vectorB)
    {
        int n = vectorB.Length;

        double[,] a = new double[n, n + 1];
        for (int r = 0; r < n; r++)
        {
            for (int c = 0; c < n; c++)
                a[r, c] = matrixA[r, c];
            a[r, n] = vectorB[r];
        }

        for (int col = 0; col < n; col++)
        {
            int pivotRow = col;
            for (int r = col + 1; r < n; r++)
                if (Math.Abs(a[r, col]) > Math.Abs(a[pivotRow, col]))
                    pivotRow = r;

            if (Math.Abs(a[pivotRow, col]) < 1e-12)
                throw new InvalidOperationException("Ведущий элемент близок к нулю: решения нет.");

            if (pivotRow != col)
                SwapRows(a, pivotRow, col, n + 1);

            double pivot = a[col, col];
            for (int c = col; c <= n; c++)
                a[col, c] /= pivot;

            for (int r = 0; r < n; r++)
            {
                if (r == col) continue;
                double factor = a[r, col];
                if (factor == 0) continue;
                for (int c = col; c <= n; c++)
                    a[r, c] -= factor * a[col, c];
            }
        }

        double[] x = new double[n];
        for (int i = 0; i < n; i++)
            x[i] = a[i, n];
        return x;
    }

    private static void SwapRows(double[,] a, int r1, int r2, int width)
    {
        for (int c = 0; c < width; c++)
            (a[r1, c], a[r2, c]) = (a[r2, c], a[r1, c]);
    }
}
