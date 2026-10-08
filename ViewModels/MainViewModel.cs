using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Drawing;
using SmartFactory.Models;
using static SmartFactory.Models.Parameters;

namespace SmartFactory.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    Robot robot;
    public double RobotX => robot.Location.X; //property die auf eine property zugreift die auf eine lokale variable zugreift
    public double RobotY => robot.Location.Y;
    public int AnzeigeAngle => robot.Angle;
    public double AnzeigeX => RobotX - robot.Borders.Left;
    public double AnzeigeY => RobotY - robot.Borders.Top;


    public MainViewModel()
    {
        Point point = new Point(48, 48);
        Rectangle borders = new Rectangle(48, 48, 505, 505);
        int angle = 0;
        robot = new Robot(point, borders, angle);

    }
    public void MoveUp()
    {
        robot.Bewegen(Direction.up);
        OnPropertyChanged(nameof(RobotY));
        OnPropertyChanged(nameof(AnzeigeY));
    }
    public void MoveDown()
    {
        robot.Bewegen(Direction.down);
        OnPropertyChanged(nameof(RobotY));
        OnPropertyChanged(nameof(AnzeigeY));
    }
    public void MoveLeft()
    {
        robot.Bewegen(Direction.left);
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(AnzeigeX));
    }
    public void MoveRight()
    {
        robot.Bewegen(Direction.right);
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(AnzeigeX));
    }   
    public void MoveRightANDup()
    {
        robot.Bewegen(Direction.rightANDup);
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(AnzeigeX));
        OnPropertyChanged(nameof(RobotY));
        OnPropertyChanged(nameof(AnzeigeY));
    }  
    public void MoveRightANDdown()
    {
        robot.Bewegen(Direction.rightANDdown);
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(AnzeigeX));
        OnPropertyChanged(nameof(RobotY));
        OnPropertyChanged(nameof(AnzeigeY));
    }  
    public void MoveLeftANDup()
    {
        robot.Bewegen(Direction.leftANDup);
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(AnzeigeX));
        OnPropertyChanged(nameof(RobotY));
        OnPropertyChanged(nameof(AnzeigeY));
    }  
    public void MoveLeftANDdown()
    {
        robot.Bewegen(Direction.leftANDdown);
        OnPropertyChanged(nameof(RobotX));
        OnPropertyChanged(nameof(AnzeigeX));
        OnPropertyChanged(nameof(RobotY));
        OnPropertyChanged(nameof(AnzeigeY));
    }  
    public void TurnLeft()
    {
        robot.Bewegen(Direction.turnLeft);
        OnPropertyChanged(nameof(AnzeigeAngle));
    }
    public void TurnRight()
    {
        robot.Bewegen(Direction.turnRight);
        OnPropertyChanged(nameof(AnzeigeAngle));
    }
}
