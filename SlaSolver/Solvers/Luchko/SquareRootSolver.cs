using SlaSolver.Solvers;
using System;

public class SquareRootSolver : ISolver
{
    public string MethodName => "Метод квадратных корней";

    public double[] Solve(double[,] matrixA, double[] vectorB)
    {
        int n = vectorB.Length;

        for (int r = 0; r < n; r++)
            for (int c = r + 1; c < n; c++)
                if (Math.Abs(matrixA[r, c] - matrixA[c, r]) > 1e-9)
                    throw new InvalidOperationException("Метод применим только к симметричной матрице.");

        double[,] l = new double[n, n];

        for (int i = 0; i < n; i++)
        {
            double sum = matrixA[i, i];
            for (int k = 0; k < i; k++)
                sum -= l[i, k] * l[i, k];

            if (sum <= 1e-12)
                throw new InvalidOperationException("Матрица не положительно определена: решения нет.");

            l[i, i] = Math.Sqrt(sum);

            for (int j = i + 1; j < n; j++)
            {
                double s = matrixA[i, j];
                for (int k = 0; k < i; k++)
                    s -= l[i, k] * l[j, k];
                l[i, j] = s / l[i, i];
            }
        }

        double[] y = new double[n];
        for (int i = 0; i < n; i++)
        {
            double s = vectorB[i];
            for (int k = 0; k < i; k++)
                s -= l[i, k] * y[k];
            y[i] = s / l[i, i];
        }

        double[] x = new double[n];
        for (int i = n - 1; i >= 0; i--)
        {
            double s = y[i];
            for (int k = i + 1; k < n; k++)
                s -= l[k, i] * x[k];
            x[i] = s / l[i, i];
        }

        return x;
    }
}
