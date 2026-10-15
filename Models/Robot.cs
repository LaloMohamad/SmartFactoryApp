using System;
using System.Drawing;
namespace SmartFactory.Models;

public class Robot : Beweger
{
    private const int RobotWidth = 70;
    private const int RobotHeight = 50;
    private double exactX;
    private double exactY;
    private double movementAngle;

    public Robot(Point location, Rectangle borders, double angle) : base(location, borders, angle)
    {
        Location = location;
        Borders = borders;
        Angle = angle;
        movementAngle = Math.Truncate(angle / 90.0) * 90;
        exactX = location.X;
        exactY = location.Y;
    }

    public Robot(Point location) : this(location, ViewParameters.Borders, 0) { }

    public void ResetAngle()
    {
        Angle = 0;
        movementAngle = 0;
    }

    public override void Bewegen(Parameters.Direction direction)
    {
        if (direction == Parameters.Direction.turnLeft)
        {
            Angle -= Parameters.rotationStep;
            UpdateMovementAngle();
            return;
        }
        if (direction == Parameters.Direction.turnRight)
        {
            Angle += Parameters.rotationStep;
            UpdateMovementAngle();
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

        // Die Tasten folgen erst bei einem vollen 90°-Schritt der Drehung.
        // Auf dem Bildschirm wächst Y nach unten.
        double radians = movementAngle * Math.PI / 180.0;
        double cosine = Math.Cos(radians);
        double sine = Math.Sin(radians);
        double deltaX = Parameters.step * (localX * cosine - localY * sine);
        double deltaY = Parameters.step * (localX * sine + localY * cosine);

        exactX = Math.Clamp(exactX + deltaX, Borders.Left, Borders.Right - RobotWidth);
        exactY = Math.Clamp(exactY + deltaY, Borders.Top, Borders.Bottom - RobotHeight);
        Location = new Point((int)Math.Round(exactX), (int)Math.Round(exactY));
    }

    private void UpdateMovementAngle()
    {
        if (Angle % 90 == 0)
            movementAngle = Angle;
    }
}
