using SlaSolver.Solvers.Ruchkins;

namespace SlaSolver.Solvers {
    public class SolverFactory 
    {
        public static IReadOnlyList<ISolver> All { get; } = new List<ISolver>
        {
            new SolverGauss(), // 3 метод
            new SolverLUDecomposition(), // 6 метод
            //сюда будем добавлять все методы решения
            
        };
    }
}
