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
    private double dockAngleOffset;

    // Labor4: Nur an der Delivery-Line darf ein Stein abgeliefert werden.
    private static readonly Rectangle DeliveryLine = new Rectangle(
        ViewParameters.DeliveryLinePosition,
        new Size(ViewParameters.DeliveryLineWidth, ViewParameters.DeliveryLineHeight));

    public double ExactX => exactX;
    public double ExactY => exactY;

    public IReadOnlyList<ProductionLine> Obstacles { get; set; } = Array.Empty<ProductionLine>();
    public LegoBrick? DockedBrick { get; private set; }

    public Robot(Point location, Rectangle borders, double angle) : base(location, borders, angle)
    {
        exactX = location.X;
        exactY = location.Y;
    }

    public Robot(Point location) : this(location, ViewParameters.Borders, 0) { }

    // The gripper is at the middle of the image's upper edge. It rotates with the robot.
    public (double X, double Y) ToolCenter => ToolCenterAt(exactX, exactY, Angle);

    public double DistanceTo(LegoBrick brick)
    {
        var tcp = ToolCenter;
        double brickX = brick.DisplayX + brick.Width / 2.0;
        double brickY = brick.DisplayY + 6;
        double dx = tcp.X - brickX;
        double dy = tcp.Y - brickY;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    public bool TryDock(LegoBrick brick)
    {
        // Labor4: Ein Stein auf der Delivery-Line oder auf dem Weg dorthin darf nicht aufgenommen werden.
        if (DockedBrick != null)
            return false;

        if ((brick.Status == Parameters.BrickStatus.OnRecycleLine && DistanceTo(brick) > 20))
            return false;

        if ((brick.Status != Parameters.BrickStatus.OnRecycleLine && DistanceTo(brick) > 10))
            return false;

        if (brick.Status != Parameters.BrickStatus.OnProductionLine &&
        brick.Status != Parameters.BrickStatus.OnRecycleLine)
            return false;

        DockedBrick = brick;
        dockAngleOffset = brick.Angle - Angle;
        brick.Dock();
        PositionDockedBrick();
        return true;
    }

    // Labor4: Erfolg oder den konkreten Fehlergrund an die Oberfläche zurückgeben.
    public Parameters.UndockResult Undock()
    {
        LegoBrick? brick = DockedBrick;
        if (brick == null)
            return Parameters.UndockResult.NoBrick;

        // Labor4: Location ist hier die linke obere Ecke; geprüft wird der Steinmittelpunkt.
        Point brickCenter = new Point((int)Math.Round(brick.DisplayX + brick.Width / 2.0),
                                      (int)Math.Round(brick.DisplayY + 6));
        if (!Auflinie(DeliveryLine, brickCenter))
            return Parameters.UndockResult.WrongPlace;

        DockedBrick = null;
        brick.Release();
        return Parameters.UndockResult.Success;
    }

    // Labor4: Toleranzbereich aus der Labor-4-Codeversion.
    private static bool Auflinie(Rectangle linie, Point p)
    {
        linie.Inflate(10, 10);
        return linie.Contains(p);
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

        // Die Tasten geben eine Richtung relativ zum gedrehten Roboter vor.
        int localX = 0;
        int localY = 0;
        switch (direction)
        {
            case Parameters.Direction.up:
                localY = -1;
                break;
            case Parameters.Direction.down:
                localY = 1;
                break;
            case Parameters.Direction.left:
                localX = -1;
                break;
            case Parameters.Direction.right:
                localX = 1;
                break;
            case Parameters.Direction.leftANDup:
                localX = -1;
                localY = -1;
                break;
            case Parameters.Direction.leftANDdown:
                localX = -1;
                localY = 1;
                break;
            case Parameters.Direction.rightANDup:
                localX = 1;
                localY = -1;
                break;
            case Parameters.Direction.rightANDdown:
                localX = 1;
                localY = 1;
                break;
            default:
                return;
        }

        // Sinus und Kosinus drehen diese Richtung in die Koordinaten der Fabrik.
        double radians = Angle * Math.PI / 180.0;
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
        PositionDockedBrick();
    }

    private void PositionDockedBrick()
    {
        if (DockedBrick == null)
            return;

        var tcp = ToolCenter;
        DockedBrick.SetPosition(tcp.X - DockedBrick.Width / 2.0,
                               tcp.Y - 6);
        DockedBrick.Angle = Angle + dockAngleOffset;
    }

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

    // Zwei gedrehte Rechtecke berühren sich nur, wenn sie auf allen vier Achsen überlappen.
    private static bool IntersectsLine(double x, double y, double angle, ProductionLine line)
    {
        double robotRadians = angle * Math.PI / 180.0;
        double lineRadians = line.Angle * Math.PI / 180.0;
        double robotCos = Math.Cos(robotRadians);
        double robotSin = Math.Sin(robotRadians);
        double lineCos = Math.Cos(lineRadians);
        double lineSin = Math.Sin(lineRadians);
        double distanceX = line.X + line.Width / 2.0 - (x + RobotWidth / 2.0);
        double distanceY = line.Y + line.Height / 2.0 - (y + RobotHeight / 2.0);
        double robotHalfWidth = RobotWidth / 2.0;
        double robotHalfHeight = RobotHeight / 2.0;
        double lineHalfWidth = line.Width / 2.0;
        double lineHalfHeight = line.Height / 2.0;
        (double X, double Y)[] axes =
        {
            (robotCos, robotSin),
            (-robotSin, robotCos),
            (lineCos, lineSin),
            (-lineSin, lineCos)
        };

        foreach (var axis in axes)
        {
            double distance = Math.Abs(distanceX * axis.X + distanceY * axis.Y);
            double robotRadius = robotHalfWidth * Math.Abs(robotCos * axis.X + robotSin * axis.Y)
                               + robotHalfHeight * Math.Abs(-robotSin * axis.X + robotCos * axis.Y);
            double lineRadius = lineHalfWidth * Math.Abs(lineCos * axis.X + lineSin * axis.Y)
                              + lineHalfHeight * Math.Abs(-lineSin * axis.X + lineCos * axis.Y);
            // Eine Lücke auf nur einer Achse schließt eine Kollision aus.
            if (distance >= robotRadius + lineRadius - 0.001)
                return false;
        }
        return true;
    }
}
