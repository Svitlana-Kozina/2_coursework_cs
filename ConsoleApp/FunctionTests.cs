using System;
using RootFinderLib.Models;

namespace ConsoleApp.Tests
{
    public static class FunctionTests
    {
        public static void Run(DataSet data, string fileName, double step = 0.5)
        {
            Console.WriteLine(new string('*', 104));
            Console.WriteLine($"\nTesting f(x), g(x), φ(x) for file: {fileName} \n");

            
            if (!string.IsNullOrWhiteSpace(data.FxString))
                Console.WriteLine($"Polynomial f(x) = {data.FxString}");
            else
                Console.WriteLine($"f(x) = {string.Join(", ", data.FxCoefficients)}");
            // Виводимо параметри
            //Console.WriteLine($"f(x) = {string.Join(", ", data.FxCoefficients)}");
            Console.WriteLine($"x0 = {data.X0}, x1 = {data.X1}, eps = {data.Epsilon}\n");

            var f = new PolynomialFunction(data.FxCoefficients);
            var g = new LagrangeFunction(data.GxPoints);

            Console.WriteLine(
                $"{"x",6}\t{"f(x)",12}\t{"g(x)",12}\t{"φ(x)=f(x)-g(x)",20}");
            Console.WriteLine(new string('-', 70));

            for (double x = data.X0; x <= data.X1 + 1e-9; x += step)
            {
                double fx = f.Evaluate(x);
                double gx = g.Evaluate(x);
                double phi = fx - gx;

                Console.WriteLine(
                    $"{x,6:F2}\t{fx,12:F6}\t{gx,12:F6}\t{phi,20:F6}");
            }

            Console.WriteLine();
        }
    }
}
