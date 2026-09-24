using SlaSolver.Solvers.Ruchkins;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlaSolver.Solvers.Karlovsk
{
    internal class SolverMatrix : ISolver
    {

        public string MethodName => "Метод Матричный";

        public double[] Solve(double[,] matrixA, double[] vectorB)
        {
            int n = matrixA.GetLength(0);
            int m = matrixA.GetLength(1);

            double[,] E = new double[n, n];
            double[,] A_revers = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                E[i, i] = 1; // создал единичную матрицу
            }
              
                             
            double[,] L = new double[n, n];
            double[,] U = new double[n, n];

            for (int k = 0; k < n; k++)
            {
                U[k, k] = 1;

                for (int i = k; i < n; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < k; j++)
                        sum += L[i, j] * U[j, k];
                    L[i, k] = matrixA[i, k] - sum;
                }

                if (Math.Abs(L[k, k]) < 1e-12)
                    throw new Exception("LU-разложение невозможно.");

                for (int j = k + 1; j < n; j++)
                {
                    double sum = 0;
                    for (int i = 0; i < k; i++)
                        sum += L[k, i] * U[i, j];
                    U[k, j] = (matrixA[k, j] - sum) / L[k, k];
                }
            }

            // === Для каждого столбца E решаем A·xo = e_i ===
            for (int col = 0; col < n; col++)
            {
                // b = col-й столбец единичной матрицы
                double[] b = new double[n];
                for (int i = 0; i < n; i++)
                    b[i] = E[i, col];

                // Прямая подстановка: L·y = b
                double[] y = new double[n];
                for (int i = 0; i < n; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < i; j++)
                        sum += L[i, j] * y[j];
                    y[i] = (b[i] - sum) / L[i, i];
                }

                // Обратная подстановка: U·xo = y
                double[] xo = new double[n];
                for (int i = n - 1; i >= 0; i--)
                {
                    double sum = 0;
                    for (int j = i + 1; j < n; j++)
                        sum += U[i, j] * xo[j];
                    xo[i] = y[i] - sum;
                }

                // Записываем xo в col-й столбец обратной матрицы
                for (int i = 0; i < n; i++)
                    A_revers[i, col] = xo[i];
            }
            
            
            if (m != vectorB.Length)
                throw new ArgumentException("Число столбцов A должно равняться длине B");

            double[] x = new double[n];

            for (int i = 0; i < n; i++)
            {
                double sum = 0;
                for (int j = 0; j < m; j++)
                    sum += A_revers[i, j] * vectorB[j];
                x[i] = sum;
            }

            return x;
        }
    }
}
       
