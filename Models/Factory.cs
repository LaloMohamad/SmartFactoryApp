using System.Collections.Generic;
using System.Drawing;

namespace SmartFactory.Models;

public class Factory
{
    private readonly Robot robot;
    private readonly List<ProductionLine> productionLines = new();
    private readonly List<LegoBrick> bricks = new();
    private LegoBrick? recentlyReleased;

    public double RobotX => robot.ExactX;
    public double RobotY => robot.ExactY;
    public double AnzeigeAngle => robot.Angle;
    public Rectangle Borders => robot.Borders;
    public IReadOnlyList<ProductionLine> ProductionLines => productionLines;
    public IReadOnlyList<LegoBrick> Bricks => bricks;
    public int TickCount { get; private set; }

    public Factory()
    {
        robot = new Robot(ViewParameters.RobotPose.Location, ViewParameters.Borders, ViewParameters.RobotPose.Angle);

        productionLines.Add(new ProductionLine(new Point(91, 183), 90, Parameters.LegoColour.red, -1));
        productionLines.Add(new ProductionLine(new Point(398, 183), 90, Parameters.LegoColour.blue, 1));
        productionLines.Add(new ProductionLine(new Point(91, 442), 0, Parameters.LegoColour.yellow, -1));
        productionLines.Add(new ProductionLine(new Point(398, 442), 0, Parameters.LegoColour.green, -1));
        productionLines.Add(new ProductionLine(new Point(248, 315), 0, Parameters.LegoColour.red, 0, ProductionLineKind.Recycle));
        productionLines.Add(new ProductionLine(new Point(210, 57), 0, Parameters.LegoColour.red, 0, ProductionLineKind.Delivery));
        robot.Obstacles = productionLines;

        foreach (ProductionLine line in productionLines)
            bricks.AddRange(line.Bricks);
    }

    public void MoveRobot(Parameters.Direction direction)
    {
        robot.Bewegen(direction);
        TryDock();
    }

    public void ResetRobotAngle()
    {
        robot.ResetAngle();
        TryDock();
    }

    public void Undock()
    {
        LegoBrick? released = robot.Undock();
        if (released == null)
            return;

        recentlyReleased = released;
        // Pull the gripper away after release so the brick stays in place.
        for (int i = 0; i < 4; i++)
            robot.Bewegen(Parameters.Direction.down);
        if (robot.DistanceTo(released) > 10.5)
            recentlyReleased = null;
    }

    public void LegoBricksBewegen()
    {
        foreach (ProductionLine line in productionLines)
            line.Bewegen();
        TryDock();
        TickCount++;
    }

    private void TryDock()
    {
        if (recentlyReleased != null && robot.DistanceTo(recentlyReleased) > 10.5)
            recentlyReleased = null;

        foreach (LegoBrick brick in bricks)
        {
            if (brick != recentlyReleased && robot.TryDock(brick))
                break;
        }
    }
}
