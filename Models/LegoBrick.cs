using System;
using System.Drawing;
using static SmartFactory.Models.Parameters;

namespace SmartFactory.Models
{
    public class LegoBrick : Beweger
    {
        private LegoColour colour;
        private LegoSize size;
        
        public Parameters.LegoColour Colour { get {return colour;} set {colour = value;} }
        public LegoSize Size { get {return size;} set {size = value;}  }
        public Pose LegoPose { get; }
        public bool Docked { get; private set; }
        public double DisplayX { get; private set; }
        public double DisplayY { get; private set; }
        public bool OnProductionLine { get; private set; } = true;
        
        public LegoBrick(Pose pose, LegoColour colour, LegoSize size) : base(pose)
        {
            Colour = colour;
            Size = size;
            LegoPose = pose;
            DisplayX = pose.X;
            DisplayY = pose.Y;
        }

        public void SetPosition(double x, double y)
        {
            DisplayX = x;
            DisplayY = y;
            Location = new Point((int)Math.Round(x), (int)Math.Round(y));
        }

        public void Dock()
        {
            Docked = true;
            OnProductionLine = false;
        }

        public void Release() => Docked = false;

    }
}
