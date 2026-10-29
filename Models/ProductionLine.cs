using System;
using System.Collections.Generic;
using System.Drawing;

namespace SmartFactory.Models;

// Eine Linie kann Steine erzeugen oder als Recycling-/Lieferstation dienen.
public enum ProductionLineKind { Colour, Recycle, Delivery }

public class ProductionLine
{
    public int Width { get; }
    public int Height { get; }

    private readonly List<LegoBrick> bricks = new();
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
        // Labor4: dieselben Maße für Anzeige und Ablieferprüfung verwenden.
        if (kind == ProductionLineKind.Delivery)
        {
            Width = ViewParameters.DeliveryLineWidth;
            Height = ViewParameters.DeliveryLineHeight;
        }
        else if (kind == ProductionLineKind.Recycle)
        {
            Width = 132;
            Height = 28;
        }
        else
        {
            Width = 105;
            Height = 18;
        }

        double radians = angle * Math.PI / 180.0;
        directionX = Math.Cos(radians);
        directionY = Math.Sin(radians);
        double normalX = -directionY * brickSide;
        double normalY = directionX * brickSide;
        double centerX = location.X + Width / 2.0;
        double centerY = location.Y + Height / 2.0;

        int brickCount = 0;
        if (kind == ProductionLineKind.Colour)
            brickCount = Parameters.bricksPerLine;

        for (int i = 0; i < brickCount; i++)
        {
            int alongLine = -33 + i * 22;
            Parameters.LegoSize size;
            if (i % 2 == 0)
                size = Parameters.LegoSize.small;
            else
                size = Parameters.LegoSize.large;

            int brickWidth = 12;
            if (size == Parameters.LegoSize.large)
                brickWidth = 20;
            int x = (int)Math.Round(centerX + alongLine * directionX + 18 * normalX - brickWidth / 2.0);
            int y = (int)Math.Round(centerY + alongLine * directionY + 18 * normalY - 6);
            var pose = new Pose(x, y, angle);
            bricks.Add(new LegoBrick(pose, colour, size));
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
            if (bricks[i].Status != Parameters.BrickStatus.OnProductionLine)
                continue;

            Point start = bricks[i].LegoPose.Location;
            bricks[i].SetPosition(start.X + distance * directionX,
                                  start.Y + distance * directionY);
        }
    }

    // Labor4: Wenn ein Stein beim Kunden ankommt, belegt ein neuer Stein seinen alten Platz.
    public LegoBrick? ReplaceBrick(LegoBrick oldBrick)
    {
        int index = bricks.IndexOf(oldBrick);
        if (index < 0)
            return null;

        LegoBrick replacement = new LegoBrick(oldBrick.LegoPose, Colour, oldBrick.Size);
        bricks[index] = replacement;
        return replacement;
    }
}
