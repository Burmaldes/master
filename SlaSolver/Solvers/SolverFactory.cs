namespace SlaSolver.Solvers {
    public class SolverFactory 
    {
        public static IReadOnlyList<ISolver> All { get; } = new List<ISolver>
        {
            new SolverGauss(),
            //сюда будем добавлять все методы решения
            
        };
    }
}
