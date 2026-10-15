using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Drawing;
using System.Diagnostics;
using SmartFactory.Models;
using static SmartFactory.Models.Parameters;
using Avalonia.Controls.Converters;
using System.Collections.Generic;
using Avalonia.Threading;

namespace SmartFactory.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    Factory factory;
    DispatcherTimer timer;
    private readonly Stopwatch stopwatch = new();
    public double RobotX => factory.RobotX; //property die auf eine property zugreift die auf eine lokale variable zugreift
    public double RobotY => factory.RobotY;
    public double AnzeigeAngle => factory.AnzeigeAngle;
    public double AnzeigeX => RobotX - factory.Borders.Left;
    public double AnzeigeY => RobotY - factory.Borders.Top;
    public IReadOnlyList<ProductionLine> LinesView => factory.ProductionLines;
    public int TickCount => factory.TickCount;
    public string Laufzeit => $"{(int)stopwatch.Elapsed.TotalSeconds} s";

    //liste mit mainview und bricks
    public List<LegoBricksViewModel> BricksView { get; } = new List<LegoBricksViewModel>();


    public MainViewModel()
    {
        factory = new Factory();
        foreach (var brick in factory.Bricks)
        {
            BricksView.Add(new LegoBricksViewModel(brick));
        }
        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromMilliseconds(timerInterval);
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
        OnPropertyChanged(nameof(AnzeigeAngle));
    }
    public void MoveUp()
    {
        factory.MoveRobot(Direction.up);
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(AnzeigeX));
        OnPropertyChanged(nameof(RobotY));
        OnPropertyChanged(nameof(AnzeigeY));
    }
    public void MoveDown()
    {
        factory.MoveRobot(Direction.down);
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(AnzeigeX));
        OnPropertyChanged(nameof(RobotY));
        OnPropertyChanged(nameof(AnzeigeY));
    }
    public void MoveLeft()
    {
        factory.MoveRobot(Direction.left);
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(AnzeigeX));
        OnPropertyChanged(nameof(RobotY));
        OnPropertyChanged(nameof(AnzeigeY));
    }
    public void MoveRight()
    {
        factory.MoveRobot(Direction.right);
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(AnzeigeX));
        OnPropertyChanged(nameof(RobotY));
        OnPropertyChanged(nameof(AnzeigeY));
    }   
    public void MoveRightANDup()
    {
        factory.MoveRobot(Direction.rightANDup);
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(AnzeigeX));
        OnPropertyChanged(nameof(RobotY));
        OnPropertyChanged(nameof(AnzeigeY));
    }  
    public void MoveRightANDdown()
    {
        factory.MoveRobot(Direction.rightANDdown);
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(AnzeigeX));
        OnPropertyChanged(nameof(RobotY));
        OnPropertyChanged(nameof(AnzeigeY));
    }  
    public void MoveLeftANDup()
    {
        factory.MoveRobot(Direction.leftANDup);
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(AnzeigeX));
        OnPropertyChanged(nameof(RobotY));
        OnPropertyChanged(nameof(AnzeigeY));
    }  
    public void MoveLeftANDdown()
    {
        factory.MoveRobot(Direction.leftANDdown);
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(AnzeigeX));
        OnPropertyChanged(nameof(RobotY));
        OnPropertyChanged(nameof(AnzeigeY));
    }  
    public void TurnLeft()
    {
        factory.MoveRobot(Direction.turnLeft);
        OnPropertyChanged(nameof(AnzeigeAngle));
    }
    public void TurnRight()
    {
        factory.MoveRobot(Direction.turnRight);
        OnPropertyChanged(nameof(AnzeigeAngle));
    }
    private void NotifyRobotPoseChanged(){
        
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(RobotY));
        OnPropertyChanged(nameof(AnzeigeX));
        OnPropertyChanged(nameof(AnzeigeY));
        OnPropertyChanged(nameof(AnzeigeAngle));
    }
    private void NotifyBricksPoseChanged(){
        foreach (var brick in BricksView)
        { 
            brick.Refresh();
        }
    }

}
