using System;
using System.Drawing;
namespace SmartFactory.Models;
using SmartFactory.Models;

    public class Beweger
    {
    private const int Width = 70;
    private const int Height = 50;

    private Point location;
    private Rectangle borders;
    private double angle;

    public Point Location {
        get { return location; }
        set { location = value; }
    }
    public Rectangle Borders { get => borders; set => borders = value; }

    public double Angle {
        get { return angle; }
        set { angle = value; }
    }
    public Beweger(Point location, Rectangle borders, double angle)
    {
        this.location = location;
        this.borders = borders;
        this.angle = angle;
        //Location = location;
        //Borders = borders;
    }

    public Beweger (Point location)
    {
        Location = location;
    }
    public Beweger (Pose pose)
    {
        Location = pose.Location;
        Angle = pose.Angle;
    }
    public virtual void Bewegen(Parameters.Direction direction)
    {
        switch (direction)
        {
            case Parameters.Direction.up:
                location.Y = Math.Max(borders.Top, location.Y - Parameters.step);
                break;
            case Parameters.Direction.down:
                location.Y = Math.Min(borders.Bottom - Height, location.Y + Parameters.step);
                break;
            case Parameters.Direction.left:
                location.X = Math.Max(borders.Left, location.X - Parameters.step);
                break;
            case Parameters.Direction.right:
                location.X = Math.Min(borders.Right - Width, location.X + Parameters.step);
                break;
            case Parameters.Direction.leftANDup:
                location.Y = Math.Max(borders.Top, location.Y - Parameters.step);
                location.X = Math.Max(borders.Left, location.X - Parameters.step);
                break;
            case Parameters.Direction.leftANDdown:
                location.Y = Math.Min(borders.Bottom - Height, location.Y + Parameters.step);
                location.X = Math.Max(borders.Left, location.X - Parameters.step);
                break;
            case Parameters.Direction.rightANDup:
                location.Y = Math.Max(borders.Top, location.Y - Parameters.step);
                location.X = Math.Min(borders.Right - Width, location.X + Parameters.step);
                break;
            case Parameters.Direction.rightANDdown:
                location.Y = Math.Min(borders.Bottom - Height, location.Y + Parameters.step);
                location.X = Math.Min(borders.Right - Width, location.X + Parameters.step);
                break;
            case Parameters.Direction.turnLeft:
                Angle -= Parameters.rotationStep;
                break;
            case Parameters.Direction.turnRight:
                Angle += Parameters.rotationStep;
                break;
            case Parameters.Direction.stop:
                break;
            
        }
    }
}
