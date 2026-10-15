namespace SmartFactory.Models;

public class Parameters
{
    public enum Direction
    {
        stop = 0,
        up = 1,
        down = 2,
        left = 3,
        right = 4,
        leftANDup = 5,
        leftANDdown = 6,
        rightANDup = 7,
        rightANDdown = 8,
        turnLeft = 9,
        turnRight = 10,
    }

    public enum LegoColour
    {
        yellow = 0,
        blue = 1,
        red = 2,
        green = 3,
    }

    public enum LegoSize
    {
        small = 0,
        large = 1,
    }
    public const int bricksPerLine = 4;
    public const double rotationStep = 5;
    public const int step = 3;
    public const int timerInterval = 50; //Timer Intervall in Millisekunden
}