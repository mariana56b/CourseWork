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
        private int _sampleSize = 150;

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
            // 1) Запитати, скільки точок показати
            string inputSize = Interaction.InputBox(
                $"Скільки випадкових точок вивести на екран? (від 3 до {allPoints.Count})",
                "Кількість точок",
                _sampleSize.ToString());

            if (!int.TryParse(inputSize, out var nPoints) || nPoints < 3 || nPoints > allPoints.Count)
            {
                MessageBox.Show(
                    $"Невірне значення. Введіть ціле число від 3 до {allPoints.Count}.",
                    "Помилка вводу",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
            _sampleSize = nPoints;

            // 2) Запитати, який n‑кутник гарантувати
            string inputN = Interaction.InputBox(
                $"Який n‑кутник додати в набір точок? (ціле ≥ 3 та ≤ {_sampleSize})",
                "Гарантування n‑кутника",
                "3");

            if (!int.TryParse(inputN, out var guaranteedN)
                || guaranteedN < 3
                || guaranteedN > _sampleSize)
            {
                MessageBox.Show(
                    $"Невірне значення n‑кутника. Має бути ціле від 3 до {_sampleSize}.",
                    "Помилка вводу",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // 3) Формуємо підмножину — спочатку випадкові _sampleSize_ точок...
            points = new List<Point>(allPoints);
            SamplePoints();

            // ...а потім додамо вершини гарантованого n‑кутника:
            EnsureRegularPolygonVertices(guaranteedN);

            // 4) Показати коло й точки
            _showData = true;
            _showPolygons = false;
            allPolygons.Clear();
            pResult.Invalidate();

            tbxResult.Text = $"Відображено {_sampleSize} випадкових точок + гарантований {guaranteedN}-кутник.";
        }
        private void SamplePoints()
        {
            if (allPoints == null) return;
            if (allPoints.Count <= _sampleSize)
                points = new List<Point>(allPoints);
            else
            {
                var rnd = new Random();
                points = allPoints
                    .OrderBy(_ => rnd.Next())
                    .Take(_sampleSize)
                    .ToList();
            }
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

        /// Читає з текстового файлу circle + allPoints.
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
