using System.Drawing;

namespace SmartFactory.Models;

// Gemeinsame Daten für Roboter und Legostein. Beide bewegen sich auf eigene Weise.
public abstract class Beweger
{
    public Point Location { get; set; }
    public Rectangle Borders { get; set; }
    public double Angle { get; set; }

    public Beweger(Point location, Rectangle borders, double angle)
    {
        Location = location;
        Borders = borders;
        Angle = angle;
    }

    public Beweger(Point location) : this(location, Rectangle.Empty, 0) { }

    public Beweger(Pose pose) : this(pose.Location, Rectangle.Empty, pose.Angle) { }

    public abstract void Bewegen(Parameters.Direction direction);
}
