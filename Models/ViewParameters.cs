using System.Drawing;
namespace SmartFactory.Models
{
    public struct Pose
    {
        public int X { get; }
        public int Y { get; }
        public double Angle { get;}
        public Pose(int x, int y, double angle)
        {
            X = x;
            Y = y;
            Angle = angle;
        }
        public Point Location => new Point(X,Y);
    }
    public static class ViewParameters
    {
        public static Pose RobotPose = new Pose(285, 135, 0);
        public static Pose BrickPose = new Pose(250, 250, 45);
        public static Rectangle Borders = new Rectangle(48, 48, 505, 505);
    }
}
