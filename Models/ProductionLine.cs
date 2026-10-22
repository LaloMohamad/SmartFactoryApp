using System;
using System.Collections.Generic;
using System.Drawing;

namespace SmartFactory.Models;

public enum ProductionLineKind { Colour, Recycle, Delivery }

public class ProductionLine
{
    public int Width { get; }
    public int Height { get; }

    private readonly List<LegoBrick> bricks = new();
    private readonly Point[] startingLocations;
    private readonly double directionX;
    private readonly double directionY;
    private int tickCount;

    public Point Location { get; }
    public double Angle { get; }
    public Parameters.LegoColour Colour { get; }
    public ProductionLineKind Kind { get; }
    public IReadOnlyList<LegoBrick> Bricks => bricks;
    public int X => Location.X;
    public int Y => Location.Y;

    public ProductionLine(Point location, double angle, Parameters.LegoColour colour, int brickSide, ProductionLineKind kind = ProductionLineKind.Colour)
    {
        Location = location;
        Angle = angle;
        Colour = colour;
        Kind = kind;
        Width = kind == ProductionLineKind.Delivery ? 180 : 105;
        Height = kind == ProductionLineKind.Delivery ? 44 : 18;

        double radians = angle * Math.PI / 180.0;
        directionX = Math.Cos(radians);
        directionY = Math.Sin(radians);
        double normalX = -directionY * brickSide;
        double normalY = directionX * brickSide;
        double centerX = location.X + Width / 2.0;
        double centerY = location.Y + Height / 2.0;

        int brickCount = kind == ProductionLineKind.Colour ? Parameters.bricksPerLine : 0;
        startingLocations = new Point[brickCount];
        for (int i = 0; i < brickCount; i++)
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
        if (Kind != ProductionLineKind.Colour)
            return;
        const int ticksPerCycle = 40;
        const double amplitude = 15;
        tickCount = (tickCount + 1) % ticksPerCycle;
        double distance = amplitude * Math.Sin(2 * Math.PI * tickCount / ticksPerCycle);

        for (int i = 0; i < bricks.Count; i++)
        {
            if (!bricks[i].OnProductionLine)
                continue;

            Point start = startingLocations[i];
            bricks[i].SetPosition(start.X + distance * directionX,
                                  start.Y + distance * directionY);
        }
    }
}
