using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using RootFinderLib.Models;

namespace RootFinderWpf.Plotting
{
    public class PlotBuilder
    {
        public PlotModel BuildPlot(
            FunctionBase f,
            FunctionBase g,
            double xmin,
            double xmax,
            double step,
            List<double> roots)
        {
            var model = new PlotModel
            {
                Title = "Functions and Roots f(x) = g(x)",
                Background = OxyColors.White,
                Padding = new OxyThickness(45, 57, 82, 10)
            };

            AddAxes(model);
            AddFunctions(model, f, g, xmin, xmax, step);
            AddRoots(model, f, roots);

            return model;
        }

        private void AddAxes(PlotModel model)
        {
            model.Legends.Add(new Legend
            {
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.BottomCenter,
                LegendOrientation = LegendOrientation.Horizontal,
                LegendBackground = OxyColor.FromAColor(200, OxyColors.White),
                LegendBorder = OxyColors.LightGray
            });

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "x",
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromRgb(220, 220, 220),
                MinorGridlineStyle = LineStyle.Dot,
                MinorGridlineColor = OxyColor.FromRgb(235, 235, 235)
            });

            model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "y",
                MajorGridlineStyle = LineStyle.Solid,
                MajorGridlineColor = OxyColor.FromRgb(220, 220, 220),
                MinorGridlineStyle = LineStyle.Dot,
                MinorGridlineColor = OxyColor.FromRgb(235, 235, 235)
            });
        }

        private void AddFunctions(PlotModel model, FunctionBase f, FunctionBase g, double xmin, double xmax, double step)
        {
            var fSeries = new LineSeries
            {
                Title = "f(x)",
                Color = OxyColor.FromRgb(74, 144, 226),
                StrokeThickness = 2
            };

            var gSeries = new LineSeries
            {
                Title = "g(x)",
                Color = OxyColor.FromRgb(0, 170, 0),
                StrokeThickness = 2
            };

            for (double x = xmin; x <= xmax + 1e-9; x += step)
            {
                fSeries.Points.Add(new DataPoint(x, f.Evaluate(x)));
                gSeries.Points.Add(new DataPoint(x, g.Evaluate(x)));
            }

            model.Series.Add(fSeries);
            model.Series.Add(gSeries);
        }

        private void AddRoots(PlotModel model, FunctionBase f, List<double> roots)
        {
            var rootSeries = new ScatterSeries
            {
                Title = "root",
                MarkerType = MarkerType.Circle,
                MarkerSize = 6,
                MarkerFill = OxyColors.Black
            };

            foreach (double root in roots)
                rootSeries.Points.Add(new ScatterPoint(root, f.Evaluate(root)));

            model.Series.Add(rootSeries);

            foreach (double root in roots)
            {
                model.Annotations.Add(new OxyPlot.Annotations.TextAnnotation
                {
                    Text = root.ToString("F3"),
                    TextPosition = new DataPoint(root, f.Evaluate(root)),
                    Offset = new ScreenVector(5, -5),
                    TextColor = OxyColors.Black,
                    Stroke = OxyColors.Undefined,
                    StrokeThickness = 0,
                    FontSize = 14
                });
            }
        }
    }
}
