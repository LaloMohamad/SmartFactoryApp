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
        
        public LegoBrick(Pose pose, LegoColour colour, LegoSize size) : base(pose)
        {
            Colour = colour;
            Size = size;
            LegoPose = pose;
        }
    }
}
