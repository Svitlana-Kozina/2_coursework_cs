using System.IO;
using OxyPlot;
using OxyPlot.SkiaSharp;

namespace RootFinderWpf.IO
{
    public static class PlotExportService
    {        
        public static void SavePng(PlotModel model, string filePath)
        {
            // Білий фон моделі
            model.Background = OxyColors.White;

            var exporter = new PngExporter
            {
                Width = 1200,
                Height = 800,
                Dpi = 96
            };

            using (var stream = File.OpenWrite(filePath))
            {
                exporter.Export(model, stream);
            }
        }

        public static void SaveSvg(PlotModel model, string filePath)
        {
            // Явно вказуємо SVG експортер з SkiaSharp
            var exporter = new OxyPlot.SkiaSharp.SvgExporter
            {
                Width = 1000,
                Height = 700
            };

            using (var stream = File.Create(filePath))
            {
                exporter.Export(model, stream);
            }
        }
    }
}
