using System;
using System.Collections.Generic;
using System.Text;

namespace SlaSolver.Solvers {
    public interface ISolver {
        string MethodName { get; }
        double[] Solve(double[,] matrixA, double[] vectorB);
    }
}
