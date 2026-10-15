using System;
using System.Collections.Generic;
using System.Drawing;

namespace SmartFactory.Models;

public class ProductionLine
{
    public const int Width = 105;
    public const int Height = 18;

    private readonly List<LegoBrick> bricks = new();
    private readonly Point[] startingLocations;
    private readonly double directionX;
    private readonly double directionY;
    private int tickCount;

    public Point Location { get; }
    public double Angle { get; }
    public Parameters.LegoColour Colour { get; }
    public IReadOnlyList<LegoBrick> Bricks => bricks;
    public int X => Location.X;
    public int Y => Location.Y;

    public ProductionLine(Point location, double angle, Parameters.LegoColour colour, int brickSide)
    {
        Location = location;
        Angle = angle;
        Colour = colour;

        double radians = angle * Math.PI / 180.0;
        directionX = Math.Cos(radians);
        directionY = Math.Sin(radians);
        double normalX = -directionY * brickSide;
        double normalY = directionX * brickSide;
        double centerX = location.X + Width / 2.0;
        double centerY = location.Y + Height / 2.0;

        startingLocations = new Point[Parameters.bricksPerLine];
        for (int i = 0; i < Parameters.bricksPerLine; i++)
        {
            int alongLine = -33 + i * 22;
            var size = i % 2 == 0 ? Parameters.LegoSize.small : Parameters.LegoSize.large;
            int brickWidth = size == Parameters.LegoSize.small ? 12 : 20;
            int x = (int)Math.Round(centerX + alongLine * directionX + 18 * normalX - brickWidth / 2.0);
            int y = (int)Math.Round(centerY + alongLine * directionY + 18 * normalY - 6);
            var pose = new Pose(x, y, angle);
            bricks.Add(new LegoBrick(pose, colour, size));
            startingLocations[i] = pose.Location;
        }
    }

    public void Bewegen()
    {
        const int ticksPerCycle = 40;
        const double amplitude = 15;
        tickCount = (tickCount + 1) % ticksPerCycle;
        double distance = amplitude * Math.Sin(2 * Math.PI * tickCount / ticksPerCycle);

        for (int i = 0; i < bricks.Count; i++)
        {
            Point start = startingLocations[i];
            bricks[i].Location = new Point(
                start.X + (int)Math.Round(distance * directionX),
                start.Y + (int)Math.Round(distance * directionY));
        }
    }
}
