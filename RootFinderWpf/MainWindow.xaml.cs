using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using OxyPlot.SkiaSharp;
using SkiaSharp;
using RootFinderLib.Models;
using RootFinderLib.Services;
using RootFinderWpf.Models;

namespace RootFinderWpf
{
    public partial class MainWindow : Window
    {
        // Колекція точок g(x)
        public ObservableCollection<PointInput> Points { get; set; } = new();

        public MainWindow()
        {
            InitializeComponent();

            PointsGrid.ItemsSource = Points;

            //f(x) = x^3 - 4x
            TxtPolynomial.Text = "1 0 -4 0";
            TxtXMin.Text = "-3";
            TxtXMax.Text = "3";
            TxtStep.Text = "0.1";
            TxtEps.Text = "0.0001";

            // g(x) points default
            Points.Add(new PointInput { X = -3, Y = 0 });
            Points.Add(new PointInput { X = 0, Y = 0 });
            Points.Add(new PointInput { X = 3, Y = 0 });

            RootsList.Items.Add("Enter data and press 'Find roots'");
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            RootsList.Items.Clear();
            Points.Clear();
            Plot.Model = null;
        }

        private void BtnFindRoots_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //Поліном
                double[] coeffs = TxtPolynomial.Text
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(double.Parse)
                    .ToArray();

                var f = new PolynomialFunction(coeffs.ToList());

                //g(x) точки
                if (Points.Count < 2)
                {
                    MessageBox.Show("Please enter at least 2 points for g(x).",
                        "Input error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var g = new LagrangeFunction(
                    Points.Select(p => (p.X, p.Y)).ToList()
                );

                //Інтервал
                double xmin = double.Parse(TxtXMin.Text);
                double xmax = double.Parse(TxtXMax.Text);
                double step = double.Parse(TxtStep.Text);
                double eps = double.Parse(TxtEps.Text);

                //Пошук коренів
                var roots = RootSearchService.FindAllRoots(f, g, xmin, xmax, step, eps);

                RootsList.Items.Clear();
                if (roots.Count == 0)
                {
                    RootsList.Items.Add("No roots found.");
                }
                else
                {
                    foreach (var r in roots)
                        RootsList.Items.Add(r.ToString("F6"));
                }

                DrawPlot(f, g, xmin, xmax, step, roots);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}",
                    "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DrawPlot(FunctionBase f, FunctionBase g, double xmin, double xmax, double step,
                              System.Collections.Generic.List<double> roots)
        {
            var model = new PlotModel { Title = "Functions and Roots f(x) = g(x)" };

            //Легенда
            model.Legends.Add(new Legend
            {
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.BottomCenter,   
                LegendOrientation = LegendOrientation.Horizontal,
                LegendBackground = OxyColor.FromAColor(200, OxyColors.White),
                LegendBorder = OxyColors.LightGray
            });

            //Осі
            //model.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "x" });
            //model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "y" });

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

            //f(x)
            var seriesF = new LineSeries
            {
                Title = "f(x)",
                Color = OxyColor.FromRgb(74, 144, 226), // Блакитний
                StrokeThickness = 2
            };

            //g(x)
            var seriesG = new LineSeries
            {
                Title = "g(x)",
                Color = OxyColor.FromRgb(0, 170, 0), // Зелений
                StrokeThickness = 2
            };

            //Додаємо точки
            for (double x = xmin; x <= xmax + 1e-9; x += step)
            {
                double fx = f.Evaluate(x);
                double gx = g.Evaluate(x);

                seriesF.Points.Add(new DataPoint(x, fx));
                seriesG.Points.Add(new DataPoint(x, gx));
            }

            model.Series.Add(seriesF);
            model.Series.Add(seriesG);

            //Root points
            var rootSeries = new ScatterSeries
            {
                Title = "root",
                MarkerType = MarkerType.Circle,
                MarkerSize = 6,
                MarkerFill = OxyColors.Black
            };

            foreach (double root in roots)
            {
                rootSeries.Points.Add(new ScatterPoint(root, f.Evaluate(root)));
            }

            model.Series.Add(rootSeries);

            // --- Root labels ---
            foreach (double root in roots)
            {
                var label = new OxyPlot.Annotations.TextAnnotation
                {
                    Text = root.ToString("F3"),
                    TextPosition = new DataPoint(root, f.Evaluate(root)),
                    Offset = new ScreenVector(5, -5),
                    TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Left,
                    TextVerticalAlignment = OxyPlot.VerticalAlignment.Bottom,
                    Stroke = OxyColors.Undefined,
                    FontSize = 14,
                    TextColor = OxyColors.Black
                };

                model.Annotations.Add(label);
            }

            Plot.Model = model;
        }

            private void Menu_LoadXml_Click(object sender, RoutedEventArgs e)
            {
                var dlg = new OpenFileDialog
                {
                    Filter = "XML Files (*.xml)|*.xml",
                    Title = "Load data from XML"
                };

                if (dlg.ShowDialog() == true)
                {
                    try
                    {
                        var data = XmlDataService.Load(dlg.FileName);

                        // Підставляємо дані у UI
                        TxtPolynomial.Text = string.Join(" ", data.FxCoefficients);
                        TxtXMin.Text = data.X0.ToString();
                        TxtXMax.Text = data.X1.ToString();
                        TxtEps.Text = data.Epsilon.ToString();

                        Points.Clear();
                        foreach (var p in data.GxPoints)
                            Points.Add(new PointInput { X = p.X, Y = p.Y });

                        MessageBox.Show("XML successfully loaded.", "Success",
                                        MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error loading XML:\n" + ex.Message,
                                        "XML Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            private void Menu_SaveXml_Click(object sender, RoutedEventArgs e)
            {
                var dlg = new SaveFileDialog
                {
                    Filter = "XML Files (*.xml)|*.xml",
                    Title = "Save data to XML"
                };

                if (dlg.ShowDialog() == true)
                {
                    try
                    {
                        var data = new DataSet
                        {
                            FxString = TxtPolynomial.Text,
                            FxCoefficients = TxtPolynomial.Text.Split().Select(double.Parse).ToList(),
                            GxPoints = Points.Select(p => (p.X, p.Y)).ToList(),
                            X0 = double.Parse(TxtXMin.Text),
                            X1 = double.Parse(TxtXMax.Text),
                            Epsilon = double.Parse(TxtEps.Text)
                        };

                        XmlDataService.Save(data, dlg.FileName);

                        MessageBox.Show("XML successfully saved.", "Success",
                                        MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error saving XML:\n" + ex.Message,
                                        "XML Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            private void Menu_SaveHtml_Click(object sender, RoutedEventArgs e)
            {
                var dlg = new SaveFileDialog
                {
                    Filter = "HTML File (*.html)|*.html",
                    Title = "Save HTML report"
                };

                if (dlg.ShowDialog() == true)
                {
                    try
                    {
                        string htmlBody =
                            $"<p><b>Polynomial:</b> {TxtPolynomial.Text}</p>" +
                            $"<p><b>Interval:</b> [{TxtXMin.Text}; {TxtXMax.Text}]</p>" +
                            $"<p><b>Epsilon:</b> {TxtEps.Text}</p>" +
                            "<h3>g(x) points</h3>" +
                            "<table><tr><th>X</th><th>Y</th></tr>" +
                            string.Join("", Points.Select(p =>
                                $"<tr><td>{p.X}</td><td>{p.Y}</td></tr>")) +
                            "</table>" +
                            "<h3>Roots</h3>" +
                            string.Join("<br>", RootsList.Items.Cast<string>());

                        HtmlReportGenerator.Generate(dlg.FileName, htmlBody);

                        MessageBox.Show("HTML report saved.", "Success",
                                        MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error saving report:\n" + ex.Message,
                                        "HTML Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            private void Menu_HelpGuide_Click(object sender, RoutedEventArgs e)
            {
                MessageBox.Show(
                    "1. Enter polynomial coefficients.\n" +
                    "2. Enter interval xmin, xmax.\n" +
                    "3. Enter g(x) points.\n" +
                    "4. Click 'Find roots'.\n" +
                    "5. Save/Load XML from File menu.\n" +
                    "6. Generate report in Reports menu.",
                    "User Guide",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            private void Menu_HelpAbout_Click(object sender, RoutedEventArgs e)
            {
                MessageBox.Show("Root Finder — Chord Method\nVersion 1.0\nCreated by Kozina Svitlana Oleksandrivna",
                    "About",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            private void Menu_HelpSupport_Click(object sender, RoutedEventArgs e)
            {
                MessageBox.Show("Support e-mail: skozina.webdev@gmail.com",
                    "Support",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            private void Menu_Exit_Click(object sender, RoutedEventArgs e)
            {
                Close();
            }
        private void Menu_SavePlotPng_Click(object sender, RoutedEventArgs e)
        {
            if (Plot.Model == null)
            {
                MessageBox.Show("Plot is empty. Generate the plot first.",
                                "No plot", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dlg = new SaveFileDialog
            {
                Filter = "PNG Image (*.png)|*.png",
                Title = "Save plot as PNG"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    // Створюємо PNG рендерер
                    var exporter = new OxyPlot.SkiaSharp.PngExporter
                    {
                        Width = 1200,
                        Height = 800
                    };

                    // Зберігаємо файл
                    using (var stream = File.OpenWrite(dlg.FileName))
                    {
                        exporter.Export(Plot.Model, stream);
                    }

                    MessageBox.Show("Plot saved successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving plot:\n" + ex.Message,
                        "PNG Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void Menu_SaveSvg_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                Filter = "SVG Image (*.svg)|*.svg",
                Title = "Save plot as SVG"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    var exporter = new OxyPlot.SkiaSharp.SvgExporter
                    {
                        Width = 1000,
                        Height = 700
                    };

                    using (var stream = File.Create(dlg.FileName))
                    {
                        exporter.Export(Plot.Model, stream);
                    }

                    MessageBox.Show("SVG file saved successfully.",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving SVG:\n" + ex.Message,
                        "SVG Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }



    }
}
