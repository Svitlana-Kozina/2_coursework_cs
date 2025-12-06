using System.Collections.Generic;
using RootFinderLib.Models;

namespace RootFinderLib.Services
{
    /// <summary>
    /// Фасад: ховає всі деталі побудови функцій, різниці,
    /// вибору стратегії та сканування інтервалу.
    /// </summary>
    public class RootFindingFacade
    {
        public List<double> FindRoots(
            IEnumerable<double> coeffs,
            IEnumerable<(double x, double y)> gPoints,
            double xmin,
            double xmax,
            double step,
            double eps)
        {
            var f = FunctionFactory.CreatePolynomial(coeffs);
            var g = FunctionFactory.CreateLagrange(gPoints);
            var diff = FunctionFactory.CreateDifference(f, g);

            IRootSolver solver = new ChordSolver(diff);
            var search = new RootSearchService(solver);

            return search.FindAllRoots(diff, xmin, xmax, step, eps);
        }
    }
}
