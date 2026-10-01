using SlaSolver.Solvers;
using System;

public class CramerSolver : ISolver
{
    public string MethodName => "Метод Крамера";

    public double[] Solve(double[,] matrixA, double[] vectorB)
    {
        int n = vectorB.Length;
        double detA = Determinant(matrixA, n);

        if (Math.Abs(detA) < 1e-12)
            throw new InvalidOperationException("det(A) = 0: единственного решения нет.");

        double[] x = new double[n];

        for (int i = 0; i < n; i++)
        {
            double[,] ai = (double[,])matrixA.Clone();
            for (int r = 0; r < n; r++)
                ai[r, i] = vectorB[r];

            x[i] = Determinant(ai, n) / detA;
        }

        return x;
    }

    private double[,] Minor(double[,] m, int n, int skipRow, int skipCol)
    {
        double[,] res = new double[n - 1, n - 1];
        int r2 = 0, c2 = 0;
        for (int r = 0; r < n; r++)
        {
            if (r == skipRow) continue;
            c2 = 0;
            for (int c = 0; c < n; c++)
            {
                if (c == skipCol) continue;
                res[r2, c2] = m[r, c];
                c2++;
            }
            r2++;
        }
        return res;
    }

    private double Determinant(double[,] m, int n)
    {
        if (n == 1) return m[0, 0];
        double sum = 0;
        for (int col = 0; col < n; col++)
        {
            int sign = (col % 2 == 0) ? 1 : -1;
            sum += sign * m[0, col] * Determinant(Minor(m, n, 0, col), n - 1);
        }
        return sum;
    }
}
