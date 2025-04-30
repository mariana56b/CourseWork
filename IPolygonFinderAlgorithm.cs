using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork
{
    public interface IPolygonFinderAlgorithm
    {
        List<Point> FindPolygon(Circle circle, List<Point> points, int sides);
    }
}
