using System.Drawing;
using FluentAssertions;
using TagCloudVisualization;

namespace TagCloudVisualizationTests;

public class Tests
{
    private Point center;
    [SetUp]
    public void Setup()
    {
        center = new Point(0, 0);
    }

    [Test]
    public void CloudLayouterConstructor_ShouldCreateInstanceWithSpecificCenter()
    {
        var layouter = new CircularCloudLayouter(center);
        
        layouter.Should().NotBeNull();
    }

    [Test]
    public void PutNextRectangle_ShouldAddFirstRectangleInCenter()
    {
        var layouter = new CircularCloudLayouter(center);
        
        var rectangleSize = new Size(20,10);
        
        var rectangle = layouter.PutNextRectangle(rectangleSize);
        
        var rectanglePoint = new Point(center.X - rectangleSize.Width / 2, center.Y -  rectangleSize.Height / 2);
        
        rectangle.Location.Should().Be(rectanglePoint);
    }
    
    [Test]
    public void PutNextRectangle_ShouldAddTwoRectangles()
    {
        var layouter = new CircularCloudLayouter(center);
        
        var firstRectangleSize = new Size(20,10);
        var secondRectangleSize = new Size(10,10);
        
        var rectangle = layouter.PutNextRectangle(firstRectangleSize);
        var secondRectangle = layouter.PutNextRectangle(secondRectangleSize);
        
        rectangle.Should().NotBeNull();
        secondRectangle.Should().NotBeNull();
    }
    
    [TestCase(-20, 10)]
    [TestCase(0, 0)]
    [TestCase(0, 1)]
    [TestCase(1, 0)]
    [TestCase(1, -10)]
    [TestCase(-10, -10)]
    public void PutNextRectangle_WithNegativeParameters_ShouldThrowException(int width, int height)
    {
        var layouter = new CircularCloudLayouter(center);
        
        var rectangleSize = new Size(width,height);
        
        var act = () => layouter.PutNextRectangle(rectangleSize);
        
        act.Should().Throw<ArgumentException>().WithMessage("The size of the rectangle must be a positive number.");
    }
    
    [Test]
    public void PutNextRectangle_TwoRectangles_ShouldNotIntersect()
    {
        var layouter = new CircularCloudLayouter(center);
        var firstSize = new Size(30, 20);
        var secondSize = new Size(25, 15);

        var firstRect = layouter.PutNextRectangle(firstSize);
        var secondRect = layouter.PutNextRectangle(secondSize);
    
        firstRect.IntersectsWith(secondRect).Should().BeFalse();
    }
    
    [Test]
    public void PutNextRectangle_ManyRectangles_ShouldNotIntersect()
    {
        var layouter = new CircularCloudLayouter(center);
        var rectangles = new List<Rectangle>();

        for (int i = 1; i <= 10; i++)
        {
            var rect = layouter.PutNextRectangle(new Size(10 * i - i, 10 * i - i * 2));
            rectangles.Add(rect);
        }

        foreach (var r1 in rectangles)
            foreach (var r2 in rectangles.Where(r2 => r2 != r1))
                r1.IntersectsWith(r2).Should().BeFalse();
    }
    
    [Test]
    public void PutNextRectangle_MultipleRectangles_ShouldFormCompactCloud()
    {
        var layouter = new CircularCloudLayouter(center);
        var sizes = new[]
        {
            new Size(30, 20),
            new Size(10, 20),
            new Size(30, 10),
            new Size(15, 25)
        };
    
        var rectangles = sizes.Select(size => layouter.PutNextRectangle(size)).ToList();
        
        var bounds = GetBoundingRectangle(rectangles);
        var cloudCenter = new Point(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2);
        var distanceFromActualCenter = Distance(cloudCenter, center);
    
        distanceFromActualCenter.Should().BeLessThan(50);
    }
    
    [Test]
    public void PutNextRectangle_ManyRectangles_ShouldFormCircularShape()
    {
        var layouter = new CircularCloudLayouter(center);
        var size = new Size(20, 10);
    
        var rectangles = new List<Rectangle>();
        for (int i = 0; i < 20; i++)
            rectangles.Add(layouter.PutNextRectangle(size));
    
        var bounds = GetBoundingRectangle(rectangles);
        var width = bounds.Width;
        var height = bounds.Height;
        
        var aspectRatio = (double)Math.Max(width, height) / Math.Min(width, height);
        aspectRatio.Should().BeLessThan(1.5);
    }

    private Rectangle GetBoundingRectangle(List<Rectangle> rectangles)
    {
        if (rectangles.Count == 0)
            return Rectangle.Empty;

        var minX = rectangles.Min(r => r.Left);
        var minY = rectangles.Min(r => r.Top);
        var maxX = rectangles.Max(r => r.Right);
        var maxY = rectangles.Max(r => r.Bottom);

        return new Rectangle(minX, minY, maxX - minX, maxY - minY);
    }

    private double Distance(Point a, Point b)
    {
        return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
    }
}