using System;
using System.Collections.Generic;
using RootFinderLib.Models;

namespace RootFinderLib.Services
{
    /// <summary>
    /// Сканує інтервал і знаходить усі корені функції func(x) = 0.
    /// Використовує стратегію IRootSolver.
    /// </summary>
    public class RootSearchService
    {
        private readonly IRootSolver solver;

        public RootSearchService(IRootSolver rootSolver)
        {
            solver = rootSolver;
        }

        public List<double> FindAllRoots(
            FunctionBase func,
            double xmin,
            double xmax,
            double step,
            double eps)
        {
            List<double> roots = new List<double>();

            double xLeft = xmin;
            double xRight = xLeft + step;

            while (xRight <= xmax + 1e-12)
            {
                double f1 = func.Evaluate(xLeft);
                double f2 = func.Evaluate(xRight);

                if (Math.Abs(f1) < eps)
                    AddIfNotDuplicate(roots, xLeft, eps);

                if (Math.Abs(f2) < eps)
                    AddIfNotDuplicate(roots, xRight, eps);

                if (f1 * f2 < 0)
                {
                    try
                    {
                        double root = solver.Solve(xLeft, xRight, eps);
                        AddIfNotDuplicate(roots, root, eps);
                    }
                    catch
                    {
                        // ігноруємо, якщо метод не збігся
                    }
                }

                xLeft = xRight;
                xRight = xLeft + step;
            }

            return roots;
        }

        private static void AddIfNotDuplicate(List<double> list, double value, double eps)
        {
            if (list.Count == 0 || Math.Abs(list[^1] - value) > eps)
                list.Add(value);
        }
    }
}
