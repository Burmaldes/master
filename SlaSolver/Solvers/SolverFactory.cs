using SlaSolver.Solvers.Ruchkins;

namespace SlaSolver.Solvers {
    public class SolverFactory 
    {
        public static IReadOnlyList<ISolver> All { get; } = new List<ISolver>
        {
             new SolverMatrix(),// 1 метод
             new SolverGauss(), // 3 метод
             new SolverTridiagonal(),// 5  метод
             new SolverLUDecomposition(), // 6 метод
             //сюда будем добавлять все методы решения
            
        };
    }
}
