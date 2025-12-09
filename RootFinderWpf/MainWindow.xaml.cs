using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Win32;

using RootFinderLib.Models;
using RootFinderLib.Services;

using RootFinderWpf.Models;
using RootFinderWpf.Plotting;
using RootFinderWpf.Validation;
using RootFinderWpf.IO;

using OxyPlot;

namespace RootFinderWpf
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<PointInput> Points { get; set; } = new();

        private readonly PlotBuilder _plotBuilder = new PlotBuilder();

        public MainWindow()
        {
            InitializeComponent();

            PointsGrid.ItemsSource = Points;

            // Default UI values
            TxtPolynomial.Text = "1 0 -4 0";
            TxtXMin.Text = "-3";
            TxtXMax.Text = "3";
            TxtStep.Text = "0.1";
            TxtEps.Text = "0.0001";

            Points.Add(new PointInput { X = -3, Y = 0 });
            Points.Add(new PointInput { X = 0, Y = 0 });
            Points.Add(new PointInput { X = 3, Y = 0 });

            RootsList.Items.Add("Enter data and press 'Find roots'");
        }


        // ============================================================
        //  VALIDATION: POINT EDITING
        // ============================================================

        private void PointsGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit)
                return;

            if (e.EditingElement is not TextBox tb)
                return;

            string text = tb.Text.Trim();

            // Allow empty cell
            if (string.IsNullOrWhiteSpace(text))
                return;

            // --- numeric check ---
            if (!InputValidator.TryValidateNumber(text, out double value))
            {
                MessageBox.Show("Enter a valid numeric value.",
                    "Input error", MessageBoxButton.OK, MessageBoxImage.Error);

                tb.Text = "";
                e.Cancel = true;
                return;
            }

            // --- unique X check ---
            var column = e.Column as DataGridBoundColumn;
            var binding = column?.Binding as Binding;

            if (binding != null && binding.Path.Path == "X")
            {
                var editedPoint = e.Row.Item as PointInput;

                if (InputValidator.IsDuplicateX(Points, editedPoint, value))
                {
                    MessageBox.Show(
                        "The X values must be unique.\nDuplicate X detected.",
                        "Input error", MessageBoxButton.OK, MessageBoxImage.Warning);

                    tb.Text = "";
                    e.Cancel = true;
                }
            }
        }


        // ============================================================
        //  MAIN BUTTON: FIND ROOTS
        // ============================================================

        private void BtnFindRoots_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double[] coeffs = TxtPolynomial.Text
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(double.Parse)
                    .ToArray();

                var f = new PolynomialFunction(coeffs.ToList());

                if (Points.Count < 2)
                {
                    MessageBox.Show("Please enter at least 2 points for g(x).",
                        "Input error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var g = new LagrangeFunction(
                    Points.Select(p => (p.X, p.Y)).ToList()
                );

                double xmin = double.Parse(TxtXMin.Text);
                double xmax = double.Parse(TxtXMax.Text);
                double step = double.Parse(TxtStep.Text);
                double eps = double.Parse(TxtEps.Text);

                var roots = RootSearchService.FindAllRoots(f, g, xmin, xmax, step, eps);

                RootsList.Items.Clear();
                if (roots.Count == 0)
                    RootsList.Items.Add("No roots found.");
                else
                    foreach (var r in roots)
                        RootsList.Items.Add(r.ToString("F6"));

                Plot.Model = _plotBuilder.BuildPlot(f, g, xmin, xmax, step, roots);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}",
                    "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // ============================================================
        //  MENU: CLEAR
        // ============================================================

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            RootsList.Items.Clear();
            Points.Clear();
            Plot.Model = null;
        }


        // ============================================================
        //  MENU: LOAD XML
        // ============================================================

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

                    TxtPolynomial.Text = string.Join(" ", data.FxCoefficients);
                    TxtXMin.Text = data.X0.ToString();
                    TxtXMax.Text = data.X1.ToString();
                    TxtEps.Text = data.Epsilon.ToString();

                    Points.Clear();
                    foreach (var p in data.GxPoints)
                        Points.Add(new PointInput { X = p.X, Y = p.Y });

                    MessageBox.Show("XML successfully loaded.",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading XML:\n{ex.Message}",
                        "XML Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        // ============================================================
        //  MENU: SAVE XML
        // ============================================================

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

                    MessageBox.Show("XML successfully saved.",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving XML:\n{ex.Message}",
                        "XML Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        // ============================================================
        //  MENU: SAVE HTML REPORT
        // ============================================================

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
                    HtmlReportService.Save(
                        dlg.FileName,
                        TxtPolynomial.Text,
                        TxtXMin.Text,
                        TxtXMax.Text,
                        TxtEps.Text,
                        Points,
                        RootsList.Items.Cast<string>()
                    );

                    MessageBox.Show("HTML report saved.",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving report:\n{ex.Message}",
                        "HTML Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        // ============================================================
        //  MENU: HELP
        // ============================================================

        private void Menu_HelpGuide_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "1. Enter polynomial coefficients.\n" +
                "2. Enter interval xmin, xmax.\n" +
                "3. Enter g(x) points.\n" +
                "4. Click 'Find roots'.\n" +
                "5. Save/Load XML from File menu.\n" +
                "6. Generate report in Reports menu.",
                "User Guide", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Menu_HelpAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Root Finder — Chord Method\nVersion 1.0\nCreated by Kozina Svitlana Oleksandrivna",
                "About", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Menu_HelpSupport_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Support e-mail: skozina.webdev@gmail.com",
                "Support", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        // ============================================================
        //  MENU: EXIT
        // ============================================================

        private void Menu_Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        // ============================================================
        //  MENU: SAVE PNG / SVG
        // ============================================================

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
                    PlotExportService.SavePng(Plot.Model, dlg.FileName);

                    MessageBox.Show("Plot saved successfully!",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving plot:\n{ex.Message}",
                        "PNG Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Menu_SaveSvg_Click(object sender, RoutedEventArgs e)
        {
            if (Plot.Model == null)
            {
                MessageBox.Show("Plot is empty. Generate the plot first.",
                    "No plot", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dlg = new SaveFileDialog
            {
                Filter = "SVG Image (*.svg)|*.svg",
                Title = "Save plot as SVG"
            };

            if (dlg.ShowDialog() == true)
            {
                try
                {
                    PlotExportService.SaveSvg(Plot.Model, dlg.FileName);

                    MessageBox.Show("SVG file saved successfully.",
                        "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving SVG:\n{ex.Message}",
                        "SVG Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
