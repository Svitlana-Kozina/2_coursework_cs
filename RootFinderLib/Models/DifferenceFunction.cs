namespace RootFinderLib.Models
{
    // Функція φ(x) = f(x) - g(x)
    public class DifferenceFunction : FunctionBase
    {
        private readonly FunctionBase f;
        private readonly FunctionBase g;

        public DifferenceFunction(FunctionBase f, FunctionBase g)
        {
            this.f = f;
            this.g = g;
        }

        public override double Evaluate(double x)
            => f.Evaluate(x) - g.Evaluate(x);
    }
}
