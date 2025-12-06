using System.IO;
using System.Text;
using System.Collections.Generic;
using RootFinderLib.Models;

namespace RootFinderLib.Services
{
    public static class HtmlReportGenerator
    {
        public static void Generate(string path, string htmlBody)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<html><head><meta charset='UTF-8'><title>Report</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("h2 { color: #4A90E2; }");
            sb.AppendLine("table { border-collapse: collapse; width: 100%; }");
            sb.AppendLine("th, td { border: 1px solid #4A90E2; padding: 8px; text-align: center; }");
            sb.AppendLine("th { background-color: #4A90E2; color: white; font-weight: bold; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head><body>");

            sb.AppendLine("<h2>Root Finder - Chord Method Report f(𝑥) = g(𝑥)</h2>");
            sb.AppendLine(htmlBody);

            sb.AppendLine("</body></html>");

            File.WriteAllText(path, sb.ToString());
        }

        public static string BuildResultTable(IEnumerable<ChordTestResult> results)
        {
            StringBuilder sb = new();

            sb.AppendLine("<table>");
            sb.AppendLine("<tr><th>Test File</th><th>All Roots</th><th>Status</th></tr>");

            foreach (var r in results)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine($"<td>{r.FileName}</td>");
                sb.AppendLine($"<td>{(r.Success ? r.RootsText : "-")}</td>");
                sb.AppendLine($"<td>{(r.Success ? "OK" : "Error: " + r.ErrorMessage)}</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");

            return sb.ToString();
        }
    }
}
