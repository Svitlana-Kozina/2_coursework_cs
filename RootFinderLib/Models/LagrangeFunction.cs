using System;
using System.Collections.Generic;

namespace RootFinderLib.Models
{
    public class LagrangeFunction : FunctionBase
    {
        private readonly List<(double X, double Y)> points;
        public LagrangeFunction(List<(double X, double Y)> points)
        {
            this.points = points ?? throw new ArgumentNullException(nameof(points));
        }        
        public override double Evaluate(double x)
        {
            double result = 0.0;
            for (int i = 0; i < points.Count; i++)
            {
                double term = points[i].Y; 

                // Обчислення базисного полінома ℓᵢ(x)
                for (int j = 0; j < points.Count; j++)
                {
                    if (i != j)
                    {
                        term *= (x - points[j].X) / (points[i].X - points[j].X);
                    }
                }
                // додаємо вклад вузла у суму
                result += term;
            }

            return result;
        }
    }
}
