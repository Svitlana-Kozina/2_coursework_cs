using System;
using System.Collections.Generic;
using System.Linq;

namespace RootFinderLib.Models
{
    /// <summary>
    /// Фабрика для створення об'єктів функцій:
    /// - поліном
    /// - Лагранж
    /// - різниця функцій f - g
    /// </summary>
    public static class FunctionFactory
    {
        public static FunctionBase CreatePolynomial(IEnumerable<double> coeffs)
        {
            return new PolynomialFunction(coeffs.ToList());
        }

        public static FunctionBase CreateLagrange(
            IEnumerable<(double x, double y)> points)
        {
            return new LagrangeFunction(points.ToList());
        }

        public static FunctionBase CreateDifference(FunctionBase f, FunctionBase g)
        {
            return new DifferenceFunction(f, g);
        }
    }
}
