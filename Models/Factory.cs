using System.Collections.Generic;
using System.Drawing;

namespace SmartFactory.Models;

public class Factory
{
    private readonly Robot robot;
    private readonly List<ProductionLine> productionLines = new();
    private readonly List<LegoBrick> bricks = new();

    public double RobotX => robot.Location.X;
    public double RobotY => robot.Location.Y;
    public double AnzeigeAngle => robot.Angle;
    public Rectangle Borders => robot.Borders;
    public IReadOnlyList<ProductionLine> ProductionLines => productionLines;
    public IReadOnlyList<LegoBrick> Bricks => bricks;
    public int TickCount { get; private set; }

    public Factory()
    {
        robot = new Robot(ViewParameters.RobotPose.Location, ViewParameters.Borders, ViewParameters.RobotPose.Angle);

        productionLines.Add(new ProductionLine(new Point(80, 120), -45, Parameters.LegoColour.red, 1));
        productionLines.Add(new ProductionLine(new Point(390, 120), 45, Parameters.LegoColour.blue, 1));
        productionLines.Add(new ProductionLine(new Point(80, 455), 45, Parameters.LegoColour.yellow, -1));
        productionLines.Add(new ProductionLine(new Point(390, 455), -45, Parameters.LegoColour.green, -1));

        foreach (ProductionLine line in productionLines)
            bricks.AddRange(line.Bricks);
    }

    public void MoveRobot(Parameters.Direction direction) => robot.Bewegen(direction);

    public void ResetRobotAngle() => robot.ResetAngle();

    public void LegoBricksBewegen()
    {
        foreach (ProductionLine line in productionLines)
            line.Bewegen();
        TickCount++;
    }
}
