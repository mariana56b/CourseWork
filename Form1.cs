using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.VisualBasic;  

namespace CourseWork
{
    public partial class PolygonInCircleFinder : Form
    {
        private Circle circle;

        private List<Point> allPoints;
        private List<Point> points;

        private List<List<Point>> allPolygons = new List<List<Point>>();
        private IPolygonFinderAlgorithm algorithm;

        private bool _showData = false;
        private bool _showPolygons = false;

        public PolygonInCircleFinder()
        {
            InitializeComponent();

            var exe = Application.StartupPath;
            var txt = Path.Combine(exe, "circle_data_4500.txt");

            // 1) Зчитати всі точки (і circle) безпосередньо з файлу
            LoadDataFromFile(txt);

            // 2) Зробити початкову вибірку
            SamplePoints();

            // 3) Підписатися на Paint
            pResult.Paint += pResult_Paint;

            _showData = false;
            _showPolygons = false;
            btnPrintN_Gons.Enabled = false;
        }

        private void pResult_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Color.White);

            if (_showData && circle != null)
            {
                using (var pen = new Pen(Color.Black, 2))
                    g.DrawEllipse(pen,
                        (float)(circle.Center.X - circle.Radius),
                        (float)(circle.Center.Y - circle.Radius),
                        (float)(circle.Radius * 2),
                        (float)(circle.Radius * 2));

                foreach (var pt in points)
                    g.FillEllipse(Brushes.Red, (float)pt.X - 3, (float)pt.Y - 3, 6, 6);
            }

            if (_showPolygons)
            {
                using (var pen = new Pen(Color.Blue, 2))
                {
                    foreach (var polygon in allPolygons)
                    {
                        for (int i = 0; i < polygon.Count; i++)
                        {
                            var a = polygon[i];
                            var b = polygon[(i + 1) % polygon.Count];
                            g.DrawLine(pen, (float)a.X, (float)a.Y, (float)b.X, (float)b.Y);
                        }
                    }
                }
            }
        }

        private void btnDrawingPointsCircle_Click(object sender, EventArgs e)
        {
            // 1) Попросити користувача ввести n для гарантованого n-кутника
            string input = Interaction.InputBox(
                "Який n-кутник гарантувати в наборі точок? (ціле ≥ 3)",
                "Гарантувати n-кутник",
                "5");
            if (!int.TryParse(input, out int guaranteedN) || guaranteedN < 3)
            {
                MessageBox.Show("Введіть ціле число ≥ 3", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2) Оновити підмножину: випадкова + гарантовані вершини
            points = new List<Point>(allPoints);
            SamplePoints();
            EnsureRegularPolygonVertices(guaranteedN);

            // 3) Показати коло й точки
            _showData = true;
            _showPolygons = false;
            allPolygons.Clear();
            pResult.Invalidate();

            tbxResult.Text = $"Нова вибірка + гарантований {guaranteedN}-кутник додано.";
        }

        private void btnPrintN_Gons_Click(object sender, EventArgs e)
        {
            if (!_showData)
            {
                MessageBox.Show("Спочатку натисніть «Вивести коло та точки».");
                return;
            }
            if (points.Count < 3)
            {
                MessageBox.Show("Не достатньо точок для побудови.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (algorithm == null)
            {
                MessageBox.Show("Будь ласка, виберіть алгоритм.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var sw = Stopwatch.StartNew();
            allPolygons.Clear();
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Алгоритм: {algorithm.GetType().Name}");
            sb.AppendLine();

            int maxSides = points.Count;
            for (int n = 3; n <= maxSides; n++)
            {
                var poly = algorithm.FindPolygon(circle, points, n);
                if (poly.Count == n)
                {
                    allPolygons.Add(poly);
                    sb.AppendLine($"Знайдено {n}-кутник:");
                    sb.AppendLine(string.Join(", ", poly.Select(p => $"({p.X:0.##};{p.Y:0.##})")));
                    sb.AppendLine();
                }
            }

            sw.Stop();
            sb.AppendLine($"Час: {sw.ElapsedMilliseconds} мс");
            tbxResult.Text = sb.ToString();

            _showPolygons = true;
            pResult.Invalidate();
        }

        private void rbtCoordinateComparisonAlgorithm_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtCoordinateComparisonAlgorithm.Checked)
            {
                algorithm = new CoordinateComparisonAlgorithm();
                btnPrintN_Gons.Enabled = true;
            }
        }

        private void rbtAngleSearchAlgorithm_CheckedChanged(object sender, EventArgs e)
        {
            if (rbtAngleSearchAlgorithm.Checked)
            {
                algorithm = new AngleSearchAlgorithm();
                btnPrintN_Gons.Enabled = true;
            }
        }

        /// <summary>
        /// Читає з текстового файлу circle + allPoints.
        /// </summary>
        private void LoadDataFromFile(string filePath)
        {
            allPoints = new List<Point>();
            double cx = 0, cy = 0, r = 0;
            var rx = new Regex(@"x\s*=\s*(?<x>[-+]?\d*\.?\d+)\s*,\s*y\s*=\s*(?<y>[-+]?\d*\.?\d+)",
                               RegexOptions.IgnoreCase);

            foreach (var raw in File.ReadAllLines(filePath))
            {
                var line = raw.Trim();
                if (line.StartsWith("Center:", StringComparison.OrdinalIgnoreCase))
                {
                    var m = rx.Match(line);
                    if (m.Success)
                    {
                        cx = double.Parse(m.Groups["x"].Value, CultureInfo.InvariantCulture);
                        cy = double.Parse(m.Groups["y"].Value, CultureInfo.InvariantCulture);
                    }
                }
                else if (line.StartsWith("Radius:", StringComparison.OrdinalIgnoreCase))
                {
                    var num = line.Substring("Radius:".Length).Trim().TrimEnd(';');
                    double.TryParse(num, NumberStyles.Float, CultureInfo.InvariantCulture, out r);
                }
                else if (line.StartsWith("Point", StringComparison.OrdinalIgnoreCase))
                {
                    var m = rx.Match(line);
                    if (m.Success)
                    {
                        double x = double.Parse(m.Groups["x"].Value, CultureInfo.InvariantCulture);
                        double y = double.Parse(m.Groups["y"].Value, CultureInfo.InvariantCulture);
                        allPoints.Add(new Point(x, y));
                    }
                }
            }

            circle = new Circle(new Point(cx, cy), r);
        }

        /// Перемішує allPoints та бере перші 150
        private void SamplePoints()
        {
            if (allPoints.Count <= 150)
            {
                points = new List<Point>(allPoints);
            }
            else
            {
                var rnd = new Random();
                points = allPoints.OrderBy(_ => rnd.Next()).Take(150).ToList();
            }
        }

        /// Додає у points вершини правильного n-кутника (для гарантії, що алгоритм його знайде)
        private void EnsureRegularPolygonVertices(int n)
        {
            if (circle == null) return;
            double angleInc = 2 * Math.PI / n;
            for (int i = 0; i < n; i++)
            {
                double a = i * angleInc;
                double x = circle.Center.X + circle.Radius * Math.Cos(a);
                double y = circle.Center.Y + circle.Radius * Math.Sin(a);
                points.Add(new Point(x, y));
            }
        }
    }
}
