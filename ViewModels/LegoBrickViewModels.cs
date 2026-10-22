using CommunityToolkit.Mvvm.ComponentModel;
using SmartFactory.Models;
using Avalonia.Media;

namespace SmartFactory.ViewModels;
public partial class LegoBricksViewModel : ViewModelBase
{
    private readonly LegoBrick brick;
    [ObservableProperty] private double x;
    [ObservableProperty] private double y;
    [ObservableProperty] private double angle;
    public double Width => brick.Size == Parameters.LegoSize.small ? 12 : 20;
    public IBrush Fill => brick.Colour switch
    {
        Parameters.LegoColour.red => Brushes.Firebrick,
        Parameters.LegoColour.blue => Brushes.RoyalBlue,
        Parameters.LegoColour.yellow => Brushes.Gold,
        Parameters.LegoColour.green => Brushes.ForestGreen,
        _ => Brushes.Gray
    };
    public LegoBricksViewModel(LegoBrick brick)
    {
        this.brick = brick;
        Refresh();

    }
    public void Refresh()
    {
        X = brick.DisplayX;
        Y = brick.DisplayY;
        Angle = brick.Angle;
    }
}
