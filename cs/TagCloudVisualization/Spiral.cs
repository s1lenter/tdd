using System.Drawing;

namespace TagCloudVisualization;

public class Spiral
{
    private readonly Point center;
    private double angle;
    private readonly double step;

    public Spiral(Point center, double step)
    {
        this.center = center;
        this.step = step;
        angle = 0;
    }

    public IEnumerable<Point> GetPoints()
    {
        while (true)
        {
            var radius = step * angle;
            var x = center.X + (int)(radius * Math.Cos(angle));
            var y = center.Y + (int)(radius * Math.Sin(angle));
            
            yield return new Point(x, y);
            
            angle += step;
        }
    }
}