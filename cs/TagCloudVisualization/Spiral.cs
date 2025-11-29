using System.Drawing;

namespace TagCloudVisualization;

public class Spiral
{
    private readonly Point _center;
    private double angle;
    private readonly double _step;

    public Spiral(Point center, double step = 1)
    {
        _center = center;
        _step = step;
        angle = 0;
    }

    public IEnumerable<Point> GetPoints()
    {
        while (true)
        {
            var radius = _step * angle;
            var x = _center.X + (int)(radius * Math.Cos(angle));
            var y = _center.Y + (int)(radius * Math.Sin(angle));
            
            yield return new Point(x, y);
            
            angle += _step;
        }
    }
}