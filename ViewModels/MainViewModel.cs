using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using SmartFactory.Models;
using static SmartFactory.Models.Parameters;

namespace SmartFactory.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly Factory factory = new();
    private readonly DispatcherTimer timer;
    private readonly Stopwatch stopwatch = new();
    [ObservableProperty] private int orderYellow;
    [ObservableProperty] private int orderBlue;
    [ObservableProperty] private int orderRed;
    [ObservableProperty] private int orderGreen;
    public int DeliveredYellow => 0;
    public int DeliveredBlue => 0;
    public int DeliveredRed => 0;
    public int DeliveredGreen => 0;

    public double RobotX => factory.RobotX;
    public double RobotY => factory.RobotY;
    public double AnzeigeAngle => factory.AnzeigeAngle;
    public double AnzeigeX => Math.Round(RobotX - factory.Borders.Left);
    public double AnzeigeY => Math.Round(RobotY - factory.Borders.Top);
    public IReadOnlyList<ProductionLine> LinesView => factory.ProductionLines;
    public int TickCount => factory.TickCount;
    public string Laufzeit => $"{(int)stopwatch.Elapsed.TotalSeconds} s";
    public List<LegoBricksViewModel> BricksView { get; } = new();

    public MainViewModel()
    {
        foreach (LegoBrick brick in factory.Bricks)
            BricksView.Add(new LegoBricksViewModel(brick));

        timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(timerInterval) };
        timer.Tick += Timer_Tick;
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        factory.LegoBricksBewegen();
        NotifyBricksPoseChanged();
        OnPropertyChanged(nameof(TickCount));
        OnPropertyChanged(nameof(Laufzeit));
    }

    public void Start()
    {
        stopwatch.Start();
        timer.Start();
    }

    public void Stop()
    {
        timer.Stop();
        stopwatch.Stop();
        OnPropertyChanged(nameof(Laufzeit));
    }

    public void ResetAngle()
    {
        factory.ResetRobotAngle();
        NotifyRobotPoseChanged();
        NotifyBricksPoseChanged();
    }

    public void Undock()
    {
        factory.Undock();
        NotifyRobotPoseChanged();
        NotifyBricksPoseChanged();
    }

    public void MoveOnce(Direction direction)
    {
        factory.MoveRobot(direction);
        NotifyRobotPoseChanged();
        NotifyBricksPoseChanged();
    }

    public void MoveUp() => MoveOnce(Direction.up);
    public void MoveDown() => MoveOnce(Direction.down);
    public void MoveLeft() => MoveOnce(Direction.left);
    public void MoveRight() => MoveOnce(Direction.right);
    public void MoveRightANDup() => MoveOnce(Direction.rightANDup);
    public void MoveRightANDdown() => MoveOnce(Direction.rightANDdown);
    public void MoveLeftANDup() => MoveOnce(Direction.leftANDup);
    public void MoveLeftANDdown() => MoveOnce(Direction.leftANDdown);
    public void TurnLeft() => MoveOnce(Direction.turnLeft);
    public void TurnRight() => MoveOnce(Direction.turnRight);

    private void NotifyRobotPoseChanged()
    {
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(RobotY));
        OnPropertyChanged(nameof(AnzeigeX));
        OnPropertyChanged(nameof(AnzeigeY));
        OnPropertyChanged(nameof(AnzeigeAngle));
    }

    private void NotifyBricksPoseChanged()
    {
        foreach (LegoBricksViewModel brick in BricksView)
            brick.Refresh();
    }
}
