using System;
using System.Collections.Generic;
using System.Drawing;

namespace SmartFactory.Models;

public class Factory
{
    private readonly Robot robot;
    private readonly List<ProductionLine> productionLines = new();
    private readonly List<LegoBrick> bricks = new();
    // Verhindert, dass der gerade abgelegte Stein sofort wieder angedockt wird.
    private LegoBrick? recentlyReleased;
    // Labor4: Neue Steine wechseln zwischen gut und defekt.
    private bool naechsterZumRecycling;

    public double RobotX => robot.ExactX;
    public double RobotY => robot.ExactY;
    public double AnzeigeAngle => robot.Angle;
    public Rectangle Borders => robot.Borders;
    public IReadOnlyList<ProductionLine> ProductionLines => productionLines;
    public IReadOnlyList<LegoBrick> Bricks => bricks;
    public int TickCount { get; private set; }
    // Labor4: Tatsächlich beim Kunden angekommene Steine pro Farbe.
    public int DeliveredYellow { get; private set; }
    public int DeliveredBlue { get; private set; }
    public int DeliveredRed { get; private set; }
    public int DeliveredGreen { get; private set; }

    public Factory()
    {
        robot = new Robot(ViewParameters.RobotPose.Location, ViewParameters.Borders, ViewParameters.RobotPose.Angle);

        productionLines.Add(new ProductionLine(new Point(91, 183), 90, Parameters.LegoColour.red, -1));
        productionLines.Add(new ProductionLine(new Point(398, 183), 90, Parameters.LegoColour.blue, 1));
        productionLines.Add(new ProductionLine(new Point(91, 442), 0, Parameters.LegoColour.yellow, -1));
        productionLines.Add(new ProductionLine(new Point(398, 442), 0, Parameters.LegoColour.green, -1));
        productionLines.Add(new ProductionLine(new Point(234, 310), 0, Parameters.LegoColour.red, 0, ProductionLineKind.Recycle));
        // Labor4: Position der Delivery-Line auch für das Abliefern verwenden.
        productionLines.Add(new ProductionLine(ViewParameters.DeliveryLinePosition, 0, Parameters.LegoColour.red, 0, ProductionLineKind.Delivery));
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

    // Labor4: Nur erfolgreich abgelegte Steine erhalten eine Qualitätsentscheidung.
    public Parameters.UndockResult Undock()
    {
        LegoBrick? brick = robot.DockedBrick;
        Parameters.UndockResult result = robot.Undock();
        if (result != Parameters.UndockResult.Success || brick == null)
            return result;

        // Neue Steine wechseln ab: gut, defekt, gut, defekt ...
        // Bei einem Recyclingstein entscheidet der Reparaturversuch.
        bool zumRecycling;
        if (brick.FromRecycle)
            zumRecycling = !brick.RepairedSuccessfully;
        else
        {
            zumRecycling = naechsterZumRecycling;
            naechsterZumRecycling = !naechsterZumRecycling;
        }

        if (zumRecycling)
            brick.ZumRecycling();
        else
            brick.ZumKunden();

        recentlyReleased = brick;
        // Pull the gripper away after release so the brick stays in place.
        for (int i = 0; i < 4; i++)
            robot.Bewegen(Parameters.Direction.down);
        if (robot.DistanceTo(brick) > 10.5)
            recentlyReleased = null;

        return result;
    }

    public void LegoBricksBewegen()
    {
        // Labor4: Steine auf der Delivery-Line fahren zum Kunden oder zum Recycling.
        for (int i = 0; i < bricks.Count; i++)
        {
            LegoBrick brick = bricks[i];
            if (brick.Status == Parameters.BrickStatus.ToCustomer)
            {
                brick.Bewegen(Parameters.Direction.right);
                if (brick.AmKundenende)
                    LieferungAbschliessen(i, brick);
            }
            else if (brick.Status == Parameters.BrickStatus.ToRecycle)
            {
                brick.Bewegen(Parameters.Direction.left);
                if (brick.AmRecyclingende)
                {
                    int slot = FreierRecyclePlatz();
                    if (slot >= 0)
                    {
                        ProductionLine recycleLine = productionLines[4];
                        int spacing = recycleLine.Width / Parameters.bricksPerLine;
                        int x = recycleLine.X + slot * spacing + spacing / 2 - brick.Width / 2;
                        int y = recycleLine.Y + (recycleLine.Height - 12) / 2;
                        bool repariert = Random.Shared.Next(2) == 0;
                        brick.AufRecyclinglinie(slot, new Point(x, y), repariert);
                    }
                }
            }
        }

        foreach (ProductionLine line in productionLines)
            line.Bewegen();
        TryDock();
        TickCount++;
    }

    // Labor4: Belegte Recyclingplätze werden nicht doppelt vergeben.
    private int FreierRecyclePlatz()
    {
        for (int slot = 0; slot < Parameters.bricksPerLine; slot++)
        {
            bool frei = true;
            foreach (LegoBrick brick in bricks)
            {
                if (brick.Status == Parameters.BrickStatus.OnRecycleLine && brick.RecycleSlot == slot)
                    frei = false;
            }
            if (frei)
                return slot;
        }
        return -1;
    }

    // Labor4: Zähler erhöhen und einen neuen Stein auf der Ursprungslinie erzeugen.
    private void LieferungAbschliessen(int brickIndex, LegoBrick brick)
    {
        switch (brick.Colour)
        {
            case Parameters.LegoColour.yellow:
                DeliveredYellow++;
                break;
            case Parameters.LegoColour.blue:
                DeliveredBlue++;
                break;
            case Parameters.LegoColour.red:
                DeliveredRed++;
                break;
            case Parameters.LegoColour.green:
                DeliveredGreen++;
                break;
        }

        // Die Ursprungslinie ersetzt den ausgelieferten Stein durch einen neuen.
        foreach (ProductionLine line in productionLines)
        {
            if (line.Kind != ProductionLineKind.Colour)
                continue;

            LegoBrick? replacement = line.ReplaceBrick(brick);
            if (replacement != null)
            {
                bricks[brickIndex] = replacement;
                return;
            }
        }
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
