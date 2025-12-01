using System;
using System.Collections.Generic;

namespace RootFinderLib.Models
{
    /// <summary>
    /// Реалізація інтерполяційного полінома Лагранжа g(x),
    /// побудованого за набором вузлів (x, y).
    ///
    /// Поліном Лагранжа використовується для наближення функції g(x),
    /// коли відомі лише значення у дискретних точках.
    ///
    /// Формула базисного полінома Лагранжа:
    /// <para>ℓᵢ(x) = Π<sub>j≠i</sub> (x - xⱼ) / (xᵢ - xⱼ)</para>
    ///
    /// Тоді повний поліном:
    /// <para>g(x) = Σ yᵢ · ℓᵢ(x)</para>
    /// </summary>
    public class LagrangeFunction : FunctionBase
    {
        /// <summary>
        /// Список інтерполяційних точок (вузлів),
        /// де кожен елемент містить пару (X, Y).
        /// </summary>
        private readonly List<(double X, double Y)> points;

        /// <summary>
        /// Створює інтерполяційний поліном Лагранжа за заданим набором точок.
        /// </summary>
        /// <param name="points">
        /// Вузли інтерполяції у вигляді списку пар (X, Y).
        /// Має містити щонайменше одну точку.
        /// </param>
        public LagrangeFunction(List<(double X, double Y)> points)
        {
            this.points = points ?? throw new ArgumentNullException(nameof(points));
        }

        /// <summary>
        /// Обчислює значення інтерполяційного полінома Лагранжа у заданій точці <paramref name="x"/>.
        /// </summary>
        /// <param name="x">Аргумент, у якому потрібно обчислити g(x).</param>
        /// <returns>Обчислене значення g(x).</returns>
        public override double Evaluate(double x)
        {
            double result = 0.0;

            // Обхід усіх вузлів (i)
            for (int i = 0; i < points.Count; i++)
            {
                double term = points[i].Y; // початковий множник y_i

                // Обчислення базисного полінома ℓᵢ(x)
                for (int j = 0; j < points.Count; j++)
                {
                    if (i != j)
                    {
                        term *= (x - points[j].X) / (points[i].X - points[j].X);
                    }
                }

                result += term; // додаємо вклад вузла у суму
            }

            return result;
        }
    }
}
