using System;
using System.Collections.Generic;

namespace RootFinderLib.Models
{
    /// <summary>
    /// Представляє поліном f(x) n-го степеня, коефіцієнти якого
    /// передаються у вигляді списку.
    ///
    /// Коефіцієнти повинні бути подані у порядку спадання ступенів:
    /// <para>[aₙ, aₙ₋₁, ..., a₁, a₀]</para>
    ///
    /// Тоді поліном має вигляд:
    /// <para>f(x) = aₙ·xⁿ + aₙ₋₁·xⁿ⁻¹ + … + a₁·x + a₀</para>
    /// </summary>
    public class PolynomialFunction : FunctionBase
    {
        /// <summary>
        /// Коефіцієнти полінома у порядку від старшого ступеня до молодшого.
        /// </summary>
        private readonly List<double> coefficients;

        /// <summary>
        /// Створює поліном з набору коефіцієнтів.
        /// </summary>
        /// <param name="coefficients">
        /// Список коефіцієнтів у форматі [aₙ, aₙ₋₁, ..., a₀].
        /// Не має бути порожнім.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Генерується, якщо список коефіцієнтів порожній.
        /// </exception>
        public PolynomialFunction(List<double> coefficients)
        {
            if (coefficients == null || coefficients.Count == 0)
                throw new ArgumentException("Polynomial must have at least one coefficient.");

            this.coefficients = coefficients;
        }

        /// <summary>
        /// Обчислює значення полінома f(x) у точці <paramref name="x"/>.
        ///
        /// Формально:
        /// <para>f(x) = Σ (aᵢ · x^(n-1-i))</para>
        /// де aᵢ — i-й коефіцієнт списку.
        /// </summary>
        /// <param name="x">Аргумент, у якому потрібно обчислити f(x).</param>
        /// <returns>Обчислене значення f(x).</returns>
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
