using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork
{
    public class AngleSearchAlgorithm : IPolygonFinderAlgorithm
    {
        public List<Point> FindPolygon(Circle circle, List<Point> points, int sides)
        {
            List<Point> polygon = new List<Point>();
            List<double> angles = points.Select(p => Math.Atan2(p.Y - circle.Center.Y, p.X - circle.Center.X)).ToList();
            angles.Sort();
            double angleIncrement = 2 * Math.PI / sides;

            for (int i = 0; i < sides; i++)
            {
                double targetAngle = i * angleIncrement;
                Point closestPoint = null;
                double smallestDifference = double.MaxValue;

                foreach (var point in points)
                {
                    double pointAngle = Math.Atan2(point.Y - circle.Center.Y, point.X - circle.Center.X);
                    double angleDifference = Math.Abs(targetAngle - pointAngle);
                    if (angleDifference < smallestDifference)
                    {
                        smallestDifference = angleDifference;
                        closestPoint = point;
                    }
                }

                if (closestPoint != null && smallestDifference < 0.5)
                {
                    polygon.Add(closestPoint);
                }
            }

            return polygon.Count == sides ? polygon : new List<Point>();
        }
    }
}
