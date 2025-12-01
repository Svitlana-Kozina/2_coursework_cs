using System;
using System.Linq;
using RootFinderLib.Models;
using RootFinderLib.Services;

namespace ConsoleApp.Tests
{
    public static class ChordTests
    {
        public static ChordTestResult RunSingle(string filePath)
        {
            ChordTestResult r = new() { FileName = filePath };

            try
            {
                var data = XmlDataService.Load(filePath);
                // ВИВОДИМО ПАРАМЕТРИ XML-ТЕСТУ
                Console.WriteLine($"f(x) = {string.Join(", ", data.FxCoefficients)}");
                Console.WriteLine($"x0 = {data.X0}, x1 = {data.X1}, eps = {data.Epsilon}");
                Console.WriteLine();

                var f = new PolynomialFunction(data.FxCoefficients);
                var g = new LagrangeFunction(data.GxPoints);

                // === ПОШУК УСІХ КОРЕНІВ ===
                var roots = RootSearchService.FindAllRoots(
                    f, g,
                    data.X0, data.X1,
                    step: 0.1,
                    eps: data.Epsilon
                );

                r.Success = true;

                // Формування тексту коренів
                if (roots.Count == 0)
                {
                    r.RootsText = "No roots";
                    Console.WriteLine($"\nTest \"{filePath}\" Roots: (no roots on this interval)\n\n");
                }
                else
                {
                    r.RootsText = string.Join(", ", roots.Select(x => x.ToString("F6")));
                    Console.WriteLine($"\nTest \"{filePath}\" Roots: {r.RootsText}\n\n");
                }
            }
            catch (Exception ex)
            {
                r.Success = false;
                r.ErrorMessage = ex.Message;
                Console.WriteLine($"\nTest \"{filePath}\" ERROR: {ex.Message}\n\n");
            }

            return r;
        }
    }
}
