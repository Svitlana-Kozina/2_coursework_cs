using System;
using System.Collections.Generic;

namespace RootFinderLib.Models
{
    public class PolynomialFunction : FunctionBase
    {
        /// <summary>
        /// Коефіцієнти полінома у порядку від старшого ступеня до молодшого.
        /// </summary>
        private readonly List<double> coefficients;

        /// <summary>
        /// Створює поліном з набору коефіцієнтів.
        /// </summary>        
        public PolynomialFunction(List<double> coefficients)
        {
            if (coefficients == null || coefficients.Count == 0)
                throw new ArgumentException("Polynomial must have at least one coefficient.");

            this.coefficients = coefficients;
        }        
        public override double Evaluate(double x)
        {
            double sum = 0.0;
            int n = coefficients.Count;

            for (int i = 0; i < n; i++)
            {
                sum += coefficients[i] * Math.Pow(x, n - 1 - i);
            }

            return sum;
        }
    }
}
