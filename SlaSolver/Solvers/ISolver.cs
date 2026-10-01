namespace SlaSolver.Solvers {
    public interface ISolver {
        string MethodName { get; }
        double[] Solve(double[,] matrixA, double[] vectorB);
    }
}
