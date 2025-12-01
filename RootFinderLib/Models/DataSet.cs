using System;
using System.Collections.Generic;

namespace RootFinderLib.Models
{
    /// <summary>
    /// Представляє набір вхідних даних для задачі знаходження кореня
    /// методом хорд для рівняння f(x) = g(x).
    /// 
    /// Дані цього класу завантажуються з XML-файлу та включають:
    ///   - коефіцієнти полінома f(x),
    ///   - точки для побудови інтерполяційного полінома g(x),
    ///   - інтервал пошуку кореня [X0, X1],
    ///   - точність Epsilon.
    /// </summary>
    public class DataSet
    {
        public string FxString { get; set; }
        /// <summary>
        /// Список коефіцієнтів полінома f(x).
        /// Коефіцієнти подаються у порядку від старшого степеня до молодшого.
        /// Завантажуються із секції &lt;Fx&gt; XML-файлу.
        /// </summary>
        public List<double> FxCoefficients { get; set; } = new List<double>();

        /// <summary>
        /// Набір точок (x, y), визначених у XML-файлі в секції &lt;Gx&gt;.
        /// Використовуються для побудови інтерполяційного полінома Лагранжа g(x).
        /// </summary>
        public List<(double X, double Y)> GxPoints { get; set; } = new List<(double X, double Y)>();

        /// <summary>
        /// Ліва межа інтервалу пошуку кореня.
        /// </summary>
        public double X0 { get; set; }

        /// <summary>
        /// Права межа інтервалу пошуку кореня.
        /// </summary>
        public double X1 { get; set; }

        /// <summary>
        /// Точність обчислення кореня.
        /// Метод хорд завершить роботу, коли різниця між послідовними наближеннями
        /// стане меншою за це значення.
        /// </summary>
        public double Epsilon { get; set; }

        /// <summary>
        /// Перевіряє коректність і повноту вхідних даних.
        /// Якщо відсутні важливі елементи (коефіцієнти полінома, точки g(x),
        /// або некоректна точність), генерується виняток із поясненням.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Викидається, якщо:
        /// - відсутні коефіцієнти полінома f(x),
        /// - відсутні точки для побудови g(x),
        /// - epsilon некоректне (недодатне).
        /// </exception>
        public void Validate()
        {
            if (FxCoefficients.Count == 0)
                throw new ArgumentException("Відсутні коефіцієнти полінома f(x).");

            if (GxPoints.Count == 0)
                throw new ArgumentException("Відсутні точки для інтерполяції g(x).");

            if (Epsilon <= 0)
                throw new ArgumentException("Параметр Epsilon має бути додатним.");
        }
    }
}
