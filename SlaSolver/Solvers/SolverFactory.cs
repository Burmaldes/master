using System;
using System.Collections.Generic;
using System.Text;

namespace SlaSolver.Solvers {
    internal class SolverFactory {
        public static IReadOnlyList<ISolver> All { get; } = new List<ISolver>
    {
            //сюда будем добавлять все методы решения
        };
    }
}
