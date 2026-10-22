using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using SmartFactory.Models;
using SmartFactory.ViewModels;

namespace SmartFactory.Views;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer movementTimer = new() { Interval = TimeSpan.FromMilliseconds(50) };
    private Parameters.Direction activeDirection = Parameters.Direction.stop;
    private bool stepMode;

    public MainWindow()
    {
        InitializeComponent();
        movementTimer.Tick += (_, _) => ViewModel?.MoveOnce(activeDirection);
        Deactivated += (_, _) => StopMovement();
    }

    private MainViewModel? ViewModel => DataContext as MainViewModel;

    private void StopMovement()
    {
        movementTimer.Stop();
        activeDirection = Parameters.Direction.stop;
    }

    private void MoveMode_Checked(object sender, RoutedEventArgs e)
    {
        stepMode = false;
        StopMovement();
    }

    private void StepMode_Checked(object sender, RoutedEventArgs e)
    {
        stepMode = true;
        StopMovement();
    }

    private void DirectionCommand(Parameters.Direction direction)
    {
        if (stepMode)
        {
            ViewModel?.MoveOnce(direction);
            return;
        }

        activeDirection = direction;
        ViewModel?.MoveOnce(direction);
        movementTimer.Start();
    }

    private void MoveUp_Click(object sender, RoutedEventArgs e) => DirectionCommand(Parameters.Direction.up);
    private void MoveDown_Click(object sender, RoutedEventArgs e) => DirectionCommand(Parameters.Direction.down);
    private void MoveLeft_Click(object sender, RoutedEventArgs e) => DirectionCommand(Parameters.Direction.left);
    private void MoveRight_Click(object sender, RoutedEventArgs e) => DirectionCommand(Parameters.Direction.right);
    private void MoveRightANDup_Click(object sender, RoutedEventArgs e) => DirectionCommand(Parameters.Direction.rightANDup);
    private void MoveRightANDdown_Click(object sender, RoutedEventArgs e) => DirectionCommand(Parameters.Direction.rightANDdown);
    private void MoveLeftANDup_Click(object sender, RoutedEventArgs e) => DirectionCommand(Parameters.Direction.leftANDup);
    private void MoveLeftANDdown_Click(object sender, RoutedEventArgs e) => DirectionCommand(Parameters.Direction.leftANDdown);
    private void TurnLeft_Click(object sender, RoutedEventArgs e) => DirectionCommand(Parameters.Direction.turnLeft);
    private void TurnRight_Click(object sender, RoutedEventArgs e) => DirectionCommand(Parameters.Direction.turnRight);
    private void StopMovement_Click(object sender, RoutedEventArgs e) => StopMovement();
    private void ResetAngle_Click(object sender, RoutedEventArgs e) => ViewModel?.ResetAngle();
    private void Start_Click(object sender, RoutedEventArgs e) => ViewModel?.Start();
    private void Stop_Click(object sender, RoutedEventArgs e)
    {
        StopMovement();
        ViewModel?.Stop();
    }
    private void Undock_Click(object sender, RoutedEventArgs e)
    {
        StopMovement();
        ViewModel?.Undock();
    }
}
