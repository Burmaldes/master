using System;
using System.Collections.Generic;
using System.Text;

namespace SlaSolver.Solvers {
    public class SolverFactory 
    {
        public static IReadOnlyList<ISolver> All { get; } = new List<ISolver>
        {
            new GaussSolver(),
            //сюда будем добавлять все методы решения
            
        };
    }
}
