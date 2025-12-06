using System;

namespace RootFinderLib.Models
{
    /// <summary>
    /// Реалізація методу хорд як стратегії.
    /// Працює з однією функцією func(x) = 0.
    /// </summary>
    public class ChordSolver : IRootSolver
    {
        private readonly FunctionBase func;

        public ChordSolver(FunctionBase function)
        {
            func = function ?? throw new ArgumentNullException(nameof(function));
        }

        public double Solve(double left, double right, double eps)
        {
            double a = left;
            double b = right;

            double fa = func.Evaluate(a);
            double fb = func.Evaluate(b);

            while (Math.Abs(b - a) > eps)
            {
                double c = b - fb * (b - a) / (fb - fa);
                double fc = func.Evaluate(c);

                a = b;
                fa = fb;

                b = c;
                fb = fc;
            }

            return b;
        }
    }
}
