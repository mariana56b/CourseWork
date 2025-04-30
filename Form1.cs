using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CourseWork
{
    public partial class PolygonInCircleFinder : Form
    {
        private Circle circle;
        private List<Point> points;
        private List<List<Point>> allPolygons = new List<List<Point>>();
        private IPolygonFinderAlgorithm algorithm;

        // Нові прапори для контролю, що малювати
        private bool _showData = false;
        private bool _showPolygons = false;

        public PolygonInCircleFinder()
        {
            InitializeComponent();

            string exe = Application.StartupPath;
            string txt = Path.Combine(exe, "circle_data_4500.txt");
            string db = Path.Combine(exe, "data.sqlite");

            InitializeDatabase(db, txt);  // створюємо/перезаписуємо БД
            LoadDataFromDatabase(db);     // читаємо З БД усі 4500 точок

            SamplePoints();               // <-- тут випадкова вибірка 15–20 точок

            pResult.Paint += pResult_Paint;

            _showData = false;
            _showPolygons = false;
        }


        // 1) Малювання всього на панелі pResult
        private void pResult_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);

            // Якщо натиснуто "Вивести коло та точки"
            if (_showData)
            {
                // Малюємо коло
                if (circle != null)
                {
                    using (var pen = new Pen(Color.Black, 2))
                    {
                        g.DrawEllipse(pen,
                            (float)(circle.Center.X - circle.Radius),
                            (float)(circle.Center.Y - circle.Radius),
                            (float)(circle.Radius * 2),
                            (float)(circle.Radius * 2));
                    }
                }

                // Малюємо точки
                if (points != null)
                {
                    foreach (var pt in points)
                        g.FillEllipse(Brushes.Red, (float)pt.X - 3, (float)pt.Y - 3, 6, 6);
                }
            }

            // Якщо натиснуто "Print N-Gons"
            if (_showPolygons && allPolygons != null)
            {
                using (var polyPen = new Pen(Color.Red, 2))
                {
                    foreach (var polygon in allPolygons)
                    {
                        for (int i = 0; i < polygon.Count; i++)
                        {
                            var a = polygon[i];
                            var b = polygon[(i + 1) % polygon.Count];
                            g.DrawLine(polyPen, (float)a.X, (float)a.Y, (float)b.X, (float)b.Y);
                        }
                    }
                }
            }
        }

        // 2) Кнопка: Вивести коло та точки
        private void btnDrawingPointsCircle_Click(object sender, EventArgs e)
        {
            _showData = true;
            _showPolygons = false;
            allPolygons.Clear();
            pResult.Invalidate();
            tbxResult.Text = "Коло та точки виведено.";
        }

        // 3) Кнопка: Знайти і показати правильні n-кутники
        private void btnPrintN_Gons_Click(object sender, EventArgs e)
        {
            if (!_showData)
            {
                MessageBox.Show("Спочатку натисніть «Вивести коло та точки».", "Нагадування", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (points == null || points.Count < 3)
            {
                MessageBox.Show("Для побудови багатокутника потрібно мінімум 3 точки.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (algorithm == null)
            {
                MessageBox.Show("Будь ласка, виберіть алгоритм.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string resultText = "Вибраний алгоритм: " + algorithm.GetType().Name + "\n\n";
            allPolygons = new List<List<Point>>();

            var stopwatch = Stopwatch.StartNew();

            for (int sides = 3; sides <= points.Count; sides++)
            {
                List<Point> polygon = algorithm.FindPolygon(circle, points, sides);
                if (polygon.Count == sides)
                {
                    allPolygons.Add(polygon);
                    resultText += "Знайдено " + sides + "-кутник:\n";
                    foreach (var p in polygon)
                        resultText += "(" + p.X + ", " + p.Y + ")\n";
                    resultText += "\n";
                }
            }

            stopwatch.Stop();
            resultText += "Час виконання: " + stopwatch.ElapsedMilliseconds + " мс";
            tbxResult.Text = resultText;

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

        private void btnLoadFromFile_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "Text files (*.txt)|*.txt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadDataFromFile(ofd.FileName);
                _showData = false;
                _showPolygons = false;
                allPolygons.Clear();
                SamplePoints();    // <-- після завантаження з тексту теж беремо випадкові 15–20
                pResult.Invalidate();
                tbxResult.Text = "Дані з файлу завантажено та відібрано випадкові точки.";
            }
            ofd.Dispose();
        }

        private void LoadDataFromFile(string filePath)
        {
            points = new List<Point>();
            double cx = 0, cy = 0, radius = 0;

            // Підготуємо regex для всіх координат
            Regex coordRx = new Regex(
                @"x\s*=\s*(?<x>[-+]?\d*\.?\d+)\s*,\s*y\s*=\s*(?<y>[-+]?\d*\.?\d+)",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            foreach (string raw in File.ReadAllLines(filePath))
            {
                string line = raw.Trim();
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    continue;

                if (line.StartsWith("Center:", StringComparison.OrdinalIgnoreCase))
                {
                    var m = coordRx.Match(line);
                    if (m.Success)
                    {
                        cx = double.Parse(m.Groups["x"].Value, CultureInfo.InvariantCulture);
                        cy = double.Parse(m.Groups["y"].Value, CultureInfo.InvariantCulture);
                    }
                }
                else if (line.StartsWith("Radius:", StringComparison.OrdinalIgnoreCase))
                {
                    // шукаємо будь-яке число після слова Radius:
                    string num = line.Substring("Radius:".Length).Trim()
                                     .TrimEnd(';');
                    double.TryParse(num, NumberStyles.Float, CultureInfo.InvariantCulture, out radius);
                }
                else if (line.StartsWith("Point", StringComparison.OrdinalIgnoreCase))
                {
                    var m = coordRx.Match(line);
                    if (m.Success)
                    {
                        double x, y;
                        if (double.TryParse(m.Groups["x"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out x) &&
                            double.TryParse(m.Groups["y"].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out y))
                        {
                            points.Add(new Point(x, y));
                        }
                    }
                }
            }

            // Після парсингу створюємо коло
            circle = new Circle(new Point(cx, cy), radius);
        }

        private void InitializeDatabase(string dbFilePath, string txtFilePath)
        {
            // Якщо БД існує — видаляємо її, щоб заново імпортувати свіжі дані
            if (File.Exists(dbFilePath))
                File.Delete(dbFilePath);

            // Створюємо новий файл бази
            SQLiteConnection.CreateFile(dbFilePath);
            using (var conn = new SQLiteConnection("Data Source=" + dbFilePath + ";Version=3;"))
            {
                conn.Open();

                // Створюємо таблиці
                string ddl = @"
            CREATE TABLE Circle (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CenterX REAL,
                CenterY REAL,
                Radius REAL
            );
            CREATE TABLE Points (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                X REAL,
                Y REAL
            );";
                using (var cmd = new SQLiteCommand(ddl, conn))
                    cmd.ExecuteNonQuery();

                // Зчитуємо з текстового файлу у circle та points
                LoadDataFromFile(txtFilePath);

                // Вставляємо коло
                using (var cmd = new SQLiteCommand(
                    "INSERT INTO Circle(CenterX,CenterY,Radius) VALUES(@cx,@cy,@r)", conn))
                {
                    cmd.Parameters.AddWithValue("@cx", circle.Center.X);
                    cmd.Parameters.AddWithValue("@cy", circle.Center.Y);
                    cmd.Parameters.AddWithValue("@r", circle.Radius);
                    cmd.ExecuteNonQuery();
                }

                // Вставляємо всі точки у транзакції
                using (var cmd = new SQLiteCommand("INSERT INTO Points(X,Y) VALUES(@x,@y)", conn))
                using (var tran = conn.BeginTransaction())
                {
                    foreach (var p in points)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@x", p.X);
                        cmd.Parameters.AddWithValue("@y", p.Y);
                        cmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
            }
        }


        private void LoadDataFromDatabase(string dbFilePath)
        {
            SQLiteConnection conn = null;
            try
            {
                conn = new SQLiteConnection("Data Source=" + dbFilePath + ";Version=3;");
                conn.Open();

                using (var cmd = new SQLiteCommand("SELECT CenterX,CenterY,Radius FROM Circle LIMIT 1", conn))
                using (var rdr = cmd.ExecuteReader())
                {
                    if (rdr.Read())
                    {
                        double cx = rdr.GetDouble(0);
                        double cy = rdr.GetDouble(1);
                        double r = rdr.GetDouble(2);
                        circle = new Circle(new Point(cx, cy), r);
                    }
                }

                points = new List<Point>();
                using (var cmd = new SQLiteCommand("SELECT X,Y FROM Points", conn))
                using (var rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                        points.Add(new Point(rdr.GetDouble(0), rdr.GetDouble(1)));
                }
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        /// <summary>
        /// Перемішує points і залишає в ньому випадкову підмножину від 15 до 20 елементів.
        /// </summary>
        private void SamplePoints()
        {
            if (points == null || points.Count == 0)
                return;

            var rnd = new Random();
            // вибрати випадковий розмір вибірки від 15 до 20
            int sampleSize = rnd.Next(15, 21);

            // Fisher–Yates shuffle (можна і LINQ, але так ефективніше)
            for (int i = points.Count - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                var tmp = points[i];
                points[i] = points[j];
                points[j] = tmp;
            }

            // урізати до потрібного розміру
            if (points.Count > sampleSize)
                points = points.GetRange(0, sampleSize);
        }

    }
}
