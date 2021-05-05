using BenchmarkDotNet.Running;

namespace Grep.Performance {
    public class Run {
        static void Main(string[] args) =>
            BenchmarkSwitcher.FromAssembly(typeof(Run).Assembly).Run(args);
    }
}
