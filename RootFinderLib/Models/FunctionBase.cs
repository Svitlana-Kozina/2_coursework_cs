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
    /// </summary>
    public abstract class FunctionBase
    {
        /// <summary>
        /// Обчислює значення функції у точці <paramref name="x"/>.        
        /// </summary>        
        public abstract double Evaluate(double x);
    }
}
