using System;
using System.Drawing;
using static SmartFactory.Models.Parameters;

namespace SmartFactory.Models;

public class LegoBrick : Beweger
{
    public LegoColour Colour { get; }
    public LegoSize Size { get; }
    public int Width
    {
        get
        {
            if (Size == LegoSize.small)
                return 12;
            return 20;
        }
    }

    public Pose LegoPose { get; }
    // Labor4: Ein Zustand statt mehrerer widersprüchlicher Ja/Nein-Felder.
    public BrickStatus Status { get; private set; } = BrickStatus.OnProductionLine;
    public int RecycleSlot { get; private set; } = -1;
    public bool FromRecycle { get; private set; }
    public bool RepairedSuccessfully { get; private set; }
    public double DisplayX { get; private set; }
    public double DisplayY { get; private set; }

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
        Status = BrickStatus.Docked;
        RecycleSlot = -1;
    }

    public void Release()
    {
        Status = BrickStatus.OnDeliveryLine;
    }

    // Labor4: Die Qualitätsprüfung bestimmt den Weg nach dem Ablegen.
    public void ZumKunden()
    {
        Status = BrickStatus.ToCustomer;
    }

    public void ZumRecycling()
    {
        Status = BrickStatus.ToRecycle;
    }

    // Labor4: Der Stein wartet auf einem der vier Recyclingplätze auf den Roboter.
    public void AufRecyclinglinie(int slot, Point position, bool repariert)
    {
        RecycleSlot = slot;
        FromRecycle = true;
        RepairedSuccessfully = repariert;
        Status = BrickStatus.OnRecycleLine;
        Angle = 0;
        SetPosition(position.X, position.Y);
    }

    public bool AmKundenende
    {
        get
        {
            double rightEdge = ViewParameters.DeliveryLinePosition.X + ViewParameters.DeliveryLineWidth;
            return DisplayX + Width / 2.0 >= rightEdge - Parameters.step;
        }
    }

    public bool AmRecyclingende
    {
        get
        {
            double leftEdge = ViewParameters.DeliveryLinePosition.X;
            return DisplayX + Width / 2.0 <= leftEdge + Parameters.step;
        }
    }

    // Steine fahren auf der Delivery-Line je nach Qualitätsprüfung nach rechts oder links.
    public override void Bewegen(Direction direction)
    {
        if (Status == BrickStatus.ToCustomer && direction == Direction.right)
        {
            double rightEdge = ViewParameters.DeliveryLinePosition.X + ViewParameters.DeliveryLineWidth;
            if (DisplayX + Width / 2.0 + Parameters.step <= rightEdge)
                SetPosition(DisplayX + Parameters.step, DisplayY);
        }
        else if (Status == BrickStatus.ToRecycle && direction == Direction.left)
        {
            double leftEdge = ViewParameters.DeliveryLinePosition.X;
            if (DisplayX + Width / 2.0 - Parameters.step >= leftEdge)
                SetPosition(DisplayX - Parameters.step, DisplayY);
        }
    }
}
