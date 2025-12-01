using System;
using System.Collections.Generic;
using RootFinderLib.Models;

namespace RootFinderLib.Services
{
    /// <summary>
    /// Сервіс пошуку всіх коренів на інтервалі [xmin, xmax]
    /// шляхом сканування інтервалу дрібними кроками
    /// та запуску методу хорд на кожному знайденому підвідрізку зі зміною знаку.
    /// Також знаходить корені, коли φ(x)=0 в точці.
    /// </summary>
    public static class RootSearchService
    {
        public static List<double> FindAllRoots(
            FunctionBase f,
            FunctionBase g,
            double xmin,
            double xmax,
            double step,
            double eps)
        {
            List<double> roots = new List<double>();
            ChordSolver solver = new ChordSolver(f, g);

            double xLeft = xmin;
            double xRight = xLeft + step;

            // Основний цикл по інтервалу
            while (xRight <= xmax + 1e-12)
            {
                double f1 = f.Evaluate(xLeft) - g.Evaluate(xLeft);
                double f2 = f.Evaluate(xRight) - g.Evaluate(xRight);

                // КОРІНЬ ПРЯМО В ТОЧЦІ xLeft
                if (Math.Abs(f1) < eps)
                {
                    AddRootIfNew(roots, xLeft, eps);
                }

                // КОРІНЬ ПРЯМО В ТОЧЦІ xRight
                if (Math.Abs(f2) < eps)
                {
                    AddRootIfNew(roots, xRight, eps);
                }

                // Є зміна знаку → запускаємо метод хорд
                if (f1 * f2 < 0)
                {
                    try
                    {
                        double root = solver.Solve(xLeft, xRight, eps);
                        AddRootIfNew(roots, root, eps);
                    }
                    catch
                    {
                        // Ігноруємо, якщо не зійшлось
                    }
                }

                xLeft = xRight;
                xRight = xLeft + step;
            }

            return roots;
        }

        /// <summary>
        /// Додає корінь до списку, уникаючи дублювання (близькі значення не повторюються).
        /// </summary>
        private static void AddRootIfNew(List<double> roots, double root, double eps)
        {
            if (roots.Count == 0 || Math.Abs(root - roots[^1]) > eps)
                roots.Add(root);
        }
    }
}
