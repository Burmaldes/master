using System;
using System.Collections.Generic;
using System.Text;

namespace SlaSolver.Solvers.Karlovsk
{
    internal class SolverTridiagonal : ISolver
    {
        public string MethodName => "Метод прогонки";

        public double[] Solve(double[,] matrixA, double[] vectorB)
        {
            int n = matrixA.GetLength(0);

            if (n != matrixA.GetLength(1))
                throw new ArgumentException("Матрица должна быть квадратной");
            if (n != vectorB.Length)
                throw new ArgumentException("Длины не совпадают");

           
            double[] a = new double[n]; // под диагональ
            double[] b = new double[n]; // главная
            double[] c = new double[n]; // над диагональ
            double[] f = new double[n]; // правая часть

            for (int i = 0; i < n; i++)
            {
                b[i] = matrixA[i, i];
                f[i] = vectorB[i];

                if (i > 0) a[i] = matrixA[i, i - 1];
                if (i < n - 1) c[i] = matrixA[i, i + 1];
            }

            return SolveTridiagonal(a, b, c, f);
        }

        public static double[] SolveTridiagonal(
            double[] a, double[] b, double[] c, double[] f)
        {
            int n = f.Length;

            double[] alpha = new double[n];
            double[] beta = new double[n];
            double[] x = new double[n];

           
            double denom = b[0];
            if (Math.Abs(denom) < 1e-12)
            {
                throw new InvalidOperationException("Деление на ноль в прогонке");
            }

            alpha[0] = -c[0] / denom;
            beta[0] = f[0] / denom;

            for (int i = 1; i < n; i++)
            {
                denom = b[i] + a[i] * alpha[i - 1];

                if (Math.Abs(denom) < 1e-12)
                {
                    throw new InvalidOperationException("Деление на ноль в прогонке");
                }
                   

                alpha[i] = (i < n - 1) ? -c[i] / denom : 0;
                beta[i] = (f[i] - a[i] * beta[i - 1]) / denom;
            }

            
            x[n - 1] = beta[n - 1];

            for (int i = n - 2; i >= 0; i--)
            {
                x[i] = alpha[i] * x[i + 1] + beta[i];
            }
              

            return x;
        }
    }
}
