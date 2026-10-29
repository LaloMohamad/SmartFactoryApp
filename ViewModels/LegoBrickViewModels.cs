using CommunityToolkit.Mvvm.ComponentModel;
using SmartFactory.Models;
using Avalonia.Media;

namespace SmartFactory.ViewModels;
public partial class LegoBricksViewModel : ViewModelBase
{
    private LegoBrick brick;
    // ObservableProperty erzeugt X, Y und Angle mit Benachrichtigung für die View.
    [ObservableProperty] private double x;
    [ObservableProperty] private double y;
    [ObservableProperty] private double angle;
    public double Width => brick.Width;
    public IBrush Fill
    {
        get
        {
            switch (brick.Colour)
            {
                case Parameters.LegoColour.red:
                    return Brushes.Firebrick;
                case Parameters.LegoColour.blue:
                    return Brushes.RoyalBlue;
                case Parameters.LegoColour.yellow:
                    return Brushes.Gold;
                case Parameters.LegoColour.green:
                    return Brushes.ForestGreen;
                default:
                    return Brushes.Gray;
            }
        }
    }
    public LegoBricksViewModel(LegoBrick brick)
    {
        this.brick = brick;
        Refresh(brick);

    }
    public void Refresh(LegoBrick currentBrick)
    {
        // Eine erfolgreiche Lieferung erzeugt an derselben Stelle einen neuen Stein.
        if (brick != currentBrick)
        {
            brick = currentBrick;
            OnPropertyChanged(nameof(Width));
            OnPropertyChanged(nameof(Fill));
        }
        X = brick.DisplayX;
        Y = brick.DisplayY;
        Angle = brick.Angle;
    }
}
