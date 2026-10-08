using Avalonia.Controls;
using Avalonia.Interactivity;
using SmartFactory.ViewModels;

namespace SmartFactory.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private MainViewModel? ViewModel => DataContext as MainViewModel;
    private void MoveUp_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.MoveUp();
    }
    private void MoveDown_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.MoveDown();
    }
    private void MoveLeft_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.MoveLeft();
    }
    private void MoveRight_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.MoveRight();
    }
    private void MoveRightANDup_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.MoveRightANDup();
    }
    private void MoveRightANDdown_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.MoveRightANDdown();
    }
    private void MoveLeftANDup_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.MoveLeftANDup();
    }
    private void MoveLeftANDdown_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.MoveLeftANDdown();
    }
    private void TurnLeft_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.TurnLeft();
    }
    private void TurnRight_Click(object sender, RoutedEventArgs e)
    {
        ViewModel?.TurnRight();
    }

}

