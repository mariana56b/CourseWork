using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork
{
    public class CoordinateComparisonAlgorithm : IPolygonFinderAlgorithm
    {
        public List<Point> FindPolygon(Circle circle, List<Point> points, int sides)
        {
            List<Point> polygon = new List<Point>();
            double angleIncrement = 2 * Math.PI / sides;

            var foundPoints = new HashSet<Point>(); // Щоб уникнути повторів

            for (int i = 0; i < sides; i++)
            {
                double angle = i * angleIncrement;
                double x = circle.Center.X + circle.Radius * Math.Cos(angle);
                double y = circle.Center.Y + circle.Radius * Math.Sin(angle);
                Point theoreticalPoint = new Point(x, y);

                // Додаємо більше точності для перевірки відстані
                foreach (var point in points)
                {
                    double distance = Math.Sqrt(Math.Pow(point.X - theoreticalPoint.X, 2) + Math.Pow(point.Y - theoreticalPoint.Y, 2));
                    if (distance < 0.5 && !foundPoints.Contains(point)) 
                    {
                        polygon.Add(point);
                        foundPoints.Add(point);
                        break; 
                    }
                }
            }

            return polygon.Count == sides ? polygon : new List<Point>();
        }
    }
}
