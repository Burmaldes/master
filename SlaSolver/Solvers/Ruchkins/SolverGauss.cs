namespace SlaSolver.Solvers.Ruchkins
{
    /*
     * Метод №3
    */
    public class SolverGauss : ISolver
    {
        public string MethodName => "Метод Гаусса";

        public double[] Solve(double[,] matrixA, double[] vectorB)
        {
            int n = vectorB.Length;

            double[,] a = (double[,])matrixA.Clone();
            double[] b = (double[])vectorB.Clone();
            double eps = 1 * Math.Pow(10, -12); ; // 1*10^-12

            for (int i = 0; i < n; i++)
            {
                // Поиск главного элемента (защита от деления на ноль и погрешностей)
                int pivot = i;
                for (int row = i + 1; row < n; row++)
                {
                    if (Math.Abs(a[row, i]) > Math.Abs(a[pivot, i]))
                        pivot = row;
                }

                if (Math.Abs(a[pivot, i]) < eps)
                    throw new InvalidOperationException("Система вырождена (определитель близок к нулю).");

                // Перестановка строк
                if (pivot != i)
                {
                    for (int col = 0; col < n; col++)
                    {
                        (a[i, col], a[pivot, col]) = (a[pivot, col], a[i, col]);
                    }
                    (b[i], b[pivot]) = (b[pivot], b[i]);
                }

                // Исключение переменных из нижних строк
                for (int row = i + 1; row < n; row++)
                {
                    double factor = a[row, i] / a[i, i];
                    if (Math.Abs(factor) < eps) continue;

                    for (int col = i; col < n; col++)
                    {
                        a[row, col] -= factor * a[i, col];
                    }
                    b[row] -= factor * b[i];
                }
            }

            double[] x = new double[n];
            for (int i = n - 1; i >= 0; i--)
            {
                double sum = b[i];
                for (int j = i + 1; j < n; j++)
                {
                    sum -= a[i, j] * x[j];
                }

                x[i] = sum / a[i, i];
            }

            // Возвращаем массив корней (MainWindow сам его отформатирует)
            return x;
        }
    }
}
