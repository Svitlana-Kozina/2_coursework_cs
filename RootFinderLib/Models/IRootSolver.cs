using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RootFinderLib.Models
{
    /// <summary>
    /// Інтерфейс стратегії чисельного знаходження коренів.
    /// Реалізації можуть бути різні: метод хорд, дотичних, бісекція тощо.
    /// </summary>
    public interface IRootSolver
    {
        double Solve(double left, double right, double eps);
    }
}

