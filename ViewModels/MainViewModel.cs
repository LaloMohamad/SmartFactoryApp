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
    // ObservableProperty erzeugt die öffentlichen Order-Properties und meldet Änderungen an die View.
    [ObservableProperty] private int orderYellow;
    [ObservableProperty] private int orderBlue;
    [ObservableProperty] private int orderRed;
    [ObservableProperty] private int orderGreen;
    // Labor4: Die Lieferzähler kommen aus der Factory, die Eingabefelder bleiben editierbar.
    public int DeliveredYellow => factory.DeliveredYellow;
    public int DeliveredBlue => factory.DeliveredBlue;
    public int DeliveredRed => factory.DeliveredRed;
    public int DeliveredGreen => factory.DeliveredGreen;
    public string UndockInfo { get; private set; } = "";

    public double RobotX => factory.RobotX;
    public double RobotY => factory.RobotY;
    public double AnzeigeAngle => factory.AnzeigeAngle;
    public double AnzeigeX => Math.Round(RobotX - factory.Borders.Left);
    public double AnzeigeY => Math.Round(RobotY - factory.Borders.Top);
    public IReadOnlyList<ProductionLine> LinesView => factory.ProductionLines;
    public int TickCount => factory.TickCount;
    public string Laufzeit
    {
        get
        {
            int seconds = (int)stopwatch.Elapsed.TotalSeconds;
            return seconds + " s";
        }
    }
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
        OnPropertyChanged(nameof(DeliveredYellow));
        OnPropertyChanged(nameof(DeliveredBlue));
        OnPropertyChanged(nameof(DeliveredRed));
        OnPropertyChanged(nameof(DeliveredGreen));
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

    // Labor4: Der Undock-Knopf zeigt auch den Grund für ein abgelehntes Ablegen.
    public void Undock()
    {
        UndockResult result = factory.Undock();
        switch (result)
        {
            case UndockResult.Success:
                UndockInfo = "Stein an Delivery abgelegt";
                break;
            case UndockResult.NoBrick:
                UndockInfo = "Kein Stein am Greifer";
                break;
            case UndockResult.WrongPlace:
                UndockInfo = "Nur an Delivery ablegen";
                break;
        }
        OnPropertyChanged(nameof(UndockInfo));
        NotifyRobotPoseChanged();
        NotifyBricksPoseChanged();
    }

    public void MoveOnce(Direction direction)
    {
        factory.MoveRobot(direction);
        NotifyRobotPoseChanged();
        NotifyBricksPoseChanged();
    }

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
        for (int i = 0; i < BricksView.Count; i++)
            BricksView[i].Refresh(factory.Bricks[i]);
    }
}
