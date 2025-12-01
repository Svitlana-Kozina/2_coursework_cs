using System;
using RootFinderLib.Services;
using ConsoleApp.Tests;

namespace ConsoleApp
{
    internal static class AppRunner
    {
        public static void Run()
        {
            var xmlResults = ChordTestsAll.RunAll();

            // Після кожного XML-файлу — робимо тестування f(x), g(x)
            foreach (var r in xmlResults)
            {
                if (r.Success)
                {
                    var data = XmlDataService.Load(r.FileName);

                    FunctionTests.Run(data, r.FileName);
                }
            }

            string html = HtmlReportGenerator.BuildResultTable(xmlResults);
            HtmlReportGenerator.Generate("report.html", html);

            Console.WriteLine("\nHTML report generated: report.html\n");
        }
    }
}
