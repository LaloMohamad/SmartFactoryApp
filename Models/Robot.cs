using System;
using System.Drawing;
using System.Collections.Generic;

namespace SmartFactory.Models;

public class Robot : Beweger
{
    public const int RobotWidth = 30;
    public const int RobotHeight = 66;

    private double exactX;
    private double exactY;
    private double movementAngle;
    private double dockAngleOffset;

    public double ExactX => exactX;
    public double ExactY => exactY;

    public IReadOnlyList<ProductionLine> Obstacles { get; set; } = Array.Empty<ProductionLine>();
    public LegoBrick? DockedBrick { get; private set; }

    public Robot(Point location, Rectangle borders, double angle) : base(location, borders, angle)
    {
        exactX = location.X;
        exactY = location.Y;
        movementAngle = Math.Truncate(angle / 5.0) * 5;
    }

    public Robot(Point location) : this(location, ViewParameters.Borders, 0) { }

    // The gripper is at the middle of the image's upper edge. It rotates with the robot.
    public (double X, double Y) ToolCenter => ToolCenterAt(exactX, exactY, Angle);

    public double DistanceTo(LegoBrick brick)
    {
        var tcp = ToolCenter;
        double brickX = brick.DisplayX + BrickWidth(brick) / 2.0;
        double brickY = brick.DisplayY + 6;
        return Math.Sqrt(Math.Pow(tcp.X - brickX, 2) + Math.Pow(tcp.Y - brickY, 2));
    }

    public bool TryDock(LegoBrick brick)
    {
        if (DockedBrick != null || brick.Docked || DistanceTo(brick) > 10)
            return false;

        DockedBrick = brick;
        dockAngleOffset = brick.Angle - Angle;
        brick.Dock();
        PositionDockedBrick();
        return true;
    }

    public LegoBrick? Undock()
    {
        LegoBrick? brick = DockedBrick;
        if (brick == null)
            return null;

        DockedBrick = null;
        brick.Release();
        return brick;
    }

    public void ResetAngle() => TryRotate(0);

    public override void Bewegen(Parameters.Direction direction)
    {
        if (direction == Parameters.Direction.turnLeft)
        {
            TryRotate(Angle - Parameters.rotationStep);
            return;
        }
        if (direction == Parameters.Direction.turnRight)
        {
            TryRotate(Angle + Parameters.rotationStep);
            return;
        }

        (int localX, int localY) = direction switch
        {
            Parameters.Direction.up => (0, -1),
            Parameters.Direction.down => (0, 1),
            Parameters.Direction.left => (-1, 0),
            Parameters.Direction.right => (1, 0),
            Parameters.Direction.leftANDup => (-1, -1),
            Parameters.Direction.leftANDdown => (-1, 1),
            Parameters.Direction.rightANDup => (1, -1),
            Parameters.Direction.rightANDdown => (1, 1),
            _ => (0, 0)
        };
        if (localX == 0 && localY == 0)
            return;

        double radians = movementAngle * Math.PI / 180.0;
        double cosine = Math.Cos(radians);
        double sine = Math.Sin(radians);
        double nextX = exactX + Parameters.step * (localX * cosine - localY * sine);
        double nextY = exactY + Parameters.step * (localX * sine + localY * cosine);

        if (!WithinFactory(nextX, nextY, Angle))
            return;

        exactX = nextX;
        exactY = nextY;
        Location = new Point((int)Math.Round(exactX), (int)Math.Round(exactY));
        PositionDockedBrick();
    }

    private void TryRotate(double nextAngle)
    {
        if (!WithinFactory(exactX, exactY, nextAngle))
            return;

        Angle = nextAngle;
        if (Angle % 5 == 0)
            movementAngle = Angle;
        PositionDockedBrick();
    }

    private void PositionDockedBrick()
    {
        if (DockedBrick == null)
            return;

        var tcp = ToolCenter;
        DockedBrick.SetPosition(tcp.X - BrickWidth(DockedBrick) / 2.0,
                               tcp.Y - 6);
        DockedBrick.Angle = Angle + dockAngleOffset;
    }

    private static int BrickWidth(LegoBrick brick) =>
        brick.Size == Parameters.LegoSize.small ? 12 : 20;

    private static (double X, double Y) ToolCenterAt(double x, double y, double angle)
    {
        double radians = angle * Math.PI / 180.0;
        return (x + RobotWidth / 2.0 + RobotHeight / 2.0 * Math.Sin(radians),
                y + RobotHeight / 2.0 - RobotHeight / 2.0 * Math.Cos(radians));
    }

    private bool WithinFactory(double x, double y, double angle)
    {
        double radians = angle * Math.PI / 180.0;
        double cosine = Math.Cos(radians);
        double sine = Math.Sin(radians);
        double centerX = x + RobotWidth / 2.0;
        double centerY = y + RobotHeight / 2.0;

        foreach (double dx in new[] { -RobotWidth / 2.0, RobotWidth / 2.0 })
        foreach (double dy in new[] { -RobotHeight / 2.0, RobotHeight / 2.0 })
        {
            double cornerX = centerX + dx * cosine - dy * sine;
            double cornerY = centerY + dx * sine + dy * cosine;
            if (cornerX < Borders.Left || cornerX > Borders.Right ||
                cornerY < Borders.Top || cornerY > Borders.Bottom)
                return false;
        }
        foreach (ProductionLine line in Obstacles)
        {
            if (IntersectsLine(x, y, angle, line))
                return false;
        }
        return true;
    }

    // Separating-axis test for the two rotated rectangles (robot and line).
    private static bool IntersectsLine(double x, double y, double angle, ProductionLine line)
    {
        double robotRadians = angle * Math.PI / 180.0;
        double lineRadians = line.Angle * Math.PI / 180.0;
        double rc = Math.Cos(robotRadians), rs = Math.Sin(robotRadians);
        double lc = Math.Cos(lineRadians), ls = Math.Sin(lineRadians);
        double dx = line.X + line.Width / 2.0 - (x + RobotWidth / 2.0);
        double dy = line.Y + line.Height / 2.0 - (y + RobotHeight / 2.0);
        double robotHalfWidth = RobotWidth / 2.0, robotHalfHeight = RobotHeight / 2.0;
        double lineHalfWidth = line.Width / 2.0, lineHalfHeight = line.Height / 2.0;
        (double X, double Y)[] axes = [(rc, rs), (-rs, rc), (lc, ls), (-ls, lc)];
        foreach (var axis in axes)
        {
            double distance = Math.Abs(dx * axis.X + dy * axis.Y);
            double robotRadius = robotHalfWidth * Math.Abs(rc * axis.X + rs * axis.Y)
                               + robotHalfHeight * Math.Abs(-rs * axis.X + rc * axis.Y);
            double lineRadius = lineHalfWidth * Math.Abs(lc * axis.X + ls * axis.Y)
                              + lineHalfHeight * Math.Abs(-ls * axis.X + lc * axis.Y);
            if (distance >= robotRadius + lineRadius - 0.001)
                return false;
        }
        return true;
    }
}
