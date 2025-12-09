using System.Collections.ObjectModel;
using System.Linq;
using RootFinderWpf.Models;

namespace RootFinderWpf.Validation
{
    public static class InputValidator
    {
        public static bool TryValidateNumber(string text, out double value)
        {
            return double.TryParse(text, out value);
        }

        public static bool IsDuplicateX(ObservableCollection<PointInput> points, PointInput edited, double x)
        {
            return points.Any(p => p != edited && p.X == x);
        }
    }
}
