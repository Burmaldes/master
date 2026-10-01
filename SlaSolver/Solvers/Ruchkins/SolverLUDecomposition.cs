namespace SlaSolver.Solvers.Ruchkins
{
    /*
     * Метод №6
    */
    public class SolverLUDecomposition : ISolver
    {
        public string MethodName => "Метод LU-разложения";

        public double[] Solve(double[,] matrixA, double[] vectorB)
        {
            int n = matrixA.GetLength(0);

            double[,] L = new double[n, n];
            double[,] U = new double[n, n];

            // LU-разложение: A = L * U
            // Диагональ U равна 1
            for (int k = 0; k < n; k++)
            {
                U[k, k] = 1;

                // Вычисляем k-й столбец L
                for (int i = k; i < n; i++)
                {
                    double sum = 0;

                    for (int j = 0; j < k; j++)
                        sum += L[i, j] * U[j, k];

                    L[i, k] = matrixA[i, k] - sum;
                }

                if (Math.Abs(L[k, k]) < 1e-12)
                    throw new Exception("LU-разложение невозможно.");

                // Вычисляем k-ю строку U
                for (int j = k + 1; j < n; j++)
                {
                    double sum = 0;

                    for (int i = 0; i < k; i++)
                        sum += L[k, i] * U[i, j];

                    U[k, j] = (matrixA[k, j] - sum) / L[k, k];
                }
            }

            // Решаем L * y = b
            double[] y = new double[n];

            for (int i = 0; i < n; i++)
            {
                double sum = 0;

                for (int j = 0; j < i; j++)
                    sum += L[i, j] * y[j];

                y[i] = (vectorB[i] - sum) / L[i, i];
            }

            // Решаем U * x = y
            double[] x = new double[n];

            for (int i = n - 1; i >= 0; i--)
            {
                double sum = 0;

                for (int j = i + 1; j < n; j++)
                    sum += U[i, j] * x[j];

                x[i] = y[i] - sum;
            }

            return x;
        }
    }
}
