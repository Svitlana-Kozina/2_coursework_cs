using System.Collections.Generic;
using System.IO;
using System.Linq;
using RootFinderWpf.Models;

namespace RootFinderWpf.IO
{
    public static class HtmlReportService
    {
        public static void Save(string path, string polynomial, string xmin, string xmax, string eps,
                                IEnumerable<PointInput> points, IEnumerable<string> roots)
        {
            string html =
                $"<p><b>Polynomial:</b> {polynomial}</p>" +
                $"<p><b>Interval:</b> [{xmin}; {xmax}]</p>" +
                $"<p><b>Epsilon:</b> {eps}</p>" +
                "<h3>g(x) points</h3>" +
                "<table><tr><th>X</th><th>Y</th></tr>" +
                string.Join("", points.Select(p => $"<tr><td>{p.X}</td><td>{p.Y}</td></tr>")) +
                "</table>" +
                "<h3>Roots</h3>" +
                string.Join("<br>", roots);

            File.WriteAllText(path, html);
        }
    }
}
