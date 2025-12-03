using System;

namespace RootFinderLib.Models
{
    /// <summary>
    /// Клас реалізує метод хорд з фіксованим кінцем. 
    /// Вибір нерухомого кінця здійснюється за ознакою збіжності:
    /// f(x) * f''(x) > 0. Якщо в точці a добуток f(a)*f''(a) > 0, 
    /// то нерухомим є кінець a; якщо в точці b добуток f(b)*f''(b) > 0,
    /// то нерухомим є кінець b. 
    /// Існує еквівалентне формулювання через знак f'(x)*f''(x), яке 
    /// визначає, з якого боку здійснюється збіжність.
    /// </summary>
    public class ChordSolver
    {
        /// <summary>Функція f(x).</summary>
        private readonly FunctionBase f;

        /// <summary>Функція g(x).</summary>
        private readonly FunctionBase g;

        /// <summary>Гранична кількість дозволених ітерацій.</summary>
        private const int MAX_ITER = 10000;

        /// <summary>
        /// Кількість виконаних ітерацій методу.
        /// Використовується для звітності.
        /// </summary>
        public int IterationCount { get; private set; }

        /// <summary>
        /// Створює об’єкт методу хорд на двох функціях f(x) та g(x).
        /// </summary>
        /// <param name="f">Функція f(x), що реалізує <see cref="FunctionBase"/>.</param>
        /// <param name="g">Функція g(x), що реалізує <see cref="FunctionBase"/>.</param>
        public ChordSolver(FunctionBase f, FunctionBase g)
        {
            this.f = f;
            this.g = g;
        }

        /// <summary>
        /// Обчислює φ(x) = f(x) − g(x), тобто функцію, корінь якої потрібно знайти.
        /// </summary>
        private double Phi(double x) => f.Evaluate(x) - g.Evaluate(x);

        /// <summary>
        /// Чисельно обчислює другу похідну φ''(x) центральною різницею:
        /// φ''(x) ≈ (φ(x+h) − 2φ(x) + φ(x−h)) / h².
        /// Використовується для вибору фіксованого кінця відрізку.
        /// </summary>
        /// <param name="x">Точка, у якій обчислюється φ''(x).</param>
        /// <param name="h">Крок дискретизації.</param>
        private double SecondDerivative(double x, double h = 1e-4)
        {
            double p1 = Phi(x + h);
            double p0 = Phi(x);
            double p_1 = Phi(x - h);

            return (p1 - 2 * p0 + p_1) / (h * h);
        }

        /// <summary>
        /// Основний метод, що виконує процес наближення кореня φ(x) = 0 
        /// на заданому інтервалі [a, b] із точністю eps.
        /// 
        /// Вибір фіксованого кінця відбувається автоматично залежно від знаку
        /// φ(x) та φ''(x) згідно з теорією збіжності методу хорд.
        /// </summary>
        /// <param name="a">Ліва межа інтервалу.</param>
        /// <param name="b">Права межа інтервалу.</param>
        /// <param name="eps">Задана точність.</param>
        /// <returns>Наближений корінь рівняння f(x)=g(x).</returns>
        /// <exception cref="Exception">
        /// Викидається, якщо:
        /// <list type="bullet">
        /// <item>немає зміни знаку φ(x) на [a,b] (умова існування кореня);</item>
        /// <item>φ(x) обчислилася як NaN або Infinity;</item>
        /// <item>метод не збігся за MAX_ITER ітерацій.</item>
        /// </list>
        /// </exception>
        public double Solve(double a, double b, double eps)
        {
            IterationCount = 0;

            double fa = Phi(a);
            double fb = Phi(b);

            if (double.IsNaN(fa) || double.IsNaN(fb) ||
                double.IsInfinity(fa) || double.IsInfinity(fb))
            {
                throw new Exception("Function evaluation returned NaN/Infinity at interval endpoints.");
            }

            // Якщо випадково вже корені.
            if (Math.Abs(fa) < eps) return a;
            if (Math.Abs(fb) < eps) return b;

            if (fa * fb > 0)
                throw new Exception("There is no sign change of the function on the initial interval (fa * fb > 0).");

            // Чисельна оцінка другої похідної
            double f2a = SecondDerivative(a);
            double f2b = SecondDerivative(b);

            // Вибір фіксованої точки (правило правильної випуклості)
            bool fixedIsA;

            if (fa * f2a > 0 && fb * f2b <= 0)
                fixedIsA = true;
            else if (fb * f2b > 0 && fa * f2a <= 0)
                fixedIsA = false;
            else
                fixedIsA = Math.Abs(f2a) >= Math.Abs(f2b);

            // Ініціалізація x_fixed та x_var
            double xFixed = fixedIsA ? a : b;
            double xVar = fixedIsA ? b : a;

            double fFixed = fixedIsA ? fa : fb;
            double fVar = fixedIsA ? fb : fa;

            int iter = 0;

            Console.WriteLine(
                "{0,5}  {1,12}  {2,12}  {3,12}  {4,20}  {5,30}",
                "Iter", "xFixed", "xVariable", "xNew", "Residual f(x)-g(x)", "Chord length |xVar-xFixed|"
            );

            // Основний цикл
            while (Math.Abs(xVar - xFixed) > eps && iter < MAX_ITER)
            {
                double denom = fVar - fFixed;
                double xNew;

                // Якщо знаменник занадто малий — обчислюємо серединну точку
                if (Math.Abs(denom) < 1e-16)
                    xNew = 0.5 * (xFixed + xVar);
                else
                    xNew = xVar - fVar * (xVar - xFixed) / denom;

                double fNew = Phi(xNew);

                Console.WriteLine(
                    "{0,5}  {1,12:F8}  {2,12:F8}  {3,12:F8}  {4,20:F8}  {5,30:F8}",
                    iter, xFixed, xVar, xNew, fNew, Math.Abs(xVar - xFixed)
                );

                IterationCount++;

                if (Math.Abs(fNew) < eps)
                    return xNew;

                // Оновлюємо лише змінну точку
                xVar = xNew;
                fVar = fNew;

                if (Math.Abs(xVar - xFixed) < eps)
                    break;

                iter++;
            }

            if (iter >= MAX_ITER)
                throw new Exception("Метод хорд не збігся (перевищено максимальну кількість ітерацій).");

            return 0.5 * (xFixed + xVar);
        }
    }
}
