using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseWork
{
    public class AngleSearchAlgorithm : IPolygonFinderAlgorithm
    {
        // Допустима похибка по дузі (arc length): 0.5 пікселя
        private double GetAngularTolerance(double radius) => 0.5 / radius;
        private double Normalize(double angle)
        {
            while (angle <= -Math.PI) angle += 2 * Math.PI;
            while (angle > Math.PI) angle -= 2 * Math.PI;
            return angle;
        }

        public List<Point> FindPolygon(Circle circle, List<Point> points, int sides)
        {
            var tol = GetAngularTolerance(circle.Radius);
            double inc = 2 * Math.PI / sides;

            var pointAngles = points
                .Select(p => new {
                    P = p,
                    Angle = Math.Atan2(p.Y - circle.Center.Y, p.X - circle.Center.X)
                })
                .ToList();

            foreach (var start in pointAngles)
            {
                var polygon = new List<Point>();
                bool ok = true;

                for (int k = 0; k < sides; k++)
                {
                    double target = Normalize(start.Angle + k * inc);

                    var best = pointAngles
                        .Select(pa => new {
                            pa.P,
                            Delta = Math.Abs(Normalize(pa.Angle - target))
                        })
                        .OrderBy(x => x.Delta)
                        .First();

                    if (best.Delta <= tol)
                        polygon.Add(best.P);
                    else
                    {
                        ok = false;
                        break;
                    }
                }

                if (ok && polygon.Count == sides)
                    return polygon;
            }

            return new List<Point>();
        }
    }
}
