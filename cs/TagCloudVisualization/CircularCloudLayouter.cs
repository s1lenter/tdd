using System.Drawing;

namespace TagCloudVisualization;

public class CircularCloudLayouter
{
    private readonly Point _center;
    private readonly List<Rectangle> _rectangles;
    private readonly Spiral _spiral;
    
    public CircularCloudLayouter(Point center)
    {
        _center = center;
        _spiral = new Spiral(center);
        _rectangles = [];
    }

    public Rectangle PutNextRectangle(Size rectangleSize)
    {
        if (rectangleSize.Width <= 0 || rectangleSize.Height <= 0)
            throw new ArgumentException("The size of the rectangle must be a positive number.");
        
        if (_rectangles.Count == 0)
            return AddFirstRectangleInCenter(rectangleSize);
        
        foreach (var point in _spiral.GetPoints())
        {
            var candidateRect = new Rectangle(point, rectangleSize);

            if (!IntersectsWithAny(candidateRect) && !IsContainedInAny(candidateRect))
            {
                var replacedRect = TryMoveToCenter(candidateRect);
                
                _rectangles.Add(replacedRect);
                return replacedRect;
            }
        }
        
        throw new InvalidOperationException("Can't place a rectangle.");
    }
    
    private bool IntersectsWithAny(Rectangle rectangle) => _rectangles.Any(r => r.IntersectsWith(rectangle));
    
    private bool IsContainedInAny(Rectangle candidate)
    {
        if (_rectangles.Any(existing => existing.Contains(candidate)))
            return true;
    
        if (_rectangles.Any(existing => candidate.Contains(existing)))
            return true;
        
        return false;
    }

    private Rectangle AddFirstRectangleInCenter(Size rectangleSize)
    {
        var rectangleLocation = new Point(
            _center.X - rectangleSize.Width / 2, 
            _center.Y - rectangleSize.Height / 2
        );
            
        var newRectangle = new Rectangle(rectangleLocation, rectangleSize);
        _rectangles.Add(newRectangle);
        return newRectangle;
    }
    
    private Rectangle TryMoveToCenter(Rectangle candidate)
    {
        var current = candidate;
        var direction = new Point(_center.X - current.X, _center.Y - current.Y);
    
        var stepX = direction.X == 0 ? 0 : direction.X / Math.Abs(direction.X);
        var stepY = direction.Y == 0 ? 0 : direction.Y / Math.Abs(direction.Y);
    
        while (CanMoveCloser(current, stepX, stepY))
        {
            var newLocation = new Point(current.X + stepX, current.Y + stepY);
            var newCandidate = new Rectangle(newLocation, current.Size);
        
            if (!IntersectsWithAny(newCandidate)  && !IsContainedInAny(newCandidate))
                current = newCandidate;
            else
                break;
        
            if (Math.Abs(current.X - candidate.X) > 100 || Math.Abs(current.Y - candidate.Y) > 100)
                break;
        }
    
        return current;
    }

    private bool CanMoveCloser(Rectangle rect, int stepX, int stepY)
    {
        var currentDistance = DistanceToCenter(rect);
        var newLocation = new Point(rect.X + stepX, rect.Y + stepY);
        var newRect = new Rectangle(newLocation, rect.Size);
        var newDistance = DistanceToCenter(newRect);
    
        return newDistance < currentDistance;
    }

    private double DistanceToCenter(Rectangle rect)
    {
        var rectCenter = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        return Math.Sqrt(Math.Pow(rectCenter.X - _center.X, 2) + Math.Pow(rectCenter.Y - _center.Y, 2));
    }
}