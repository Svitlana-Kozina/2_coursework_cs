namespace RootFinderLib.Models
{
    /// <summary>
    /// Абстрактний базовий клас для всіх функцій, що використовуються
    /// у задачі знаходження кореня рівняння f(x) = g(x).
    /// 
    /// Від цього класу успадковуються:
    /// <list type="bullet">
    ///   <item><description><see cref="PolynomialFunction"/> — поліном f(x)</description></item>
    ///   <item><description><see cref="LagrangeFunction"/> — інтерполяційний поліном g(x)</description></item>
    /// </list>
    /// 
    /// Містить єдиний абстрактний метод Evaluate(x), який обов’язково
    /// реалізується у класах-нащадках.
    /// </summary>
    public abstract class FunctionBase
    {
        /// <summary>
        /// Обчислює значення функції у точці <paramref name="x"/>.
        /// 
        /// Кожен клас, що успадковується від FunctionBase, повинен
        /// реалізувати власний механізм обчислення значення функції.
        /// </summary>
        /// <param name="x">Аргумент, у якому потрібно обчислити функцію.</param>
        /// <returns>Значення функції у точці x.</returns>
        public abstract double Evaluate(double x);
    }
}
