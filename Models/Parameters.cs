namespace SmartFactory.Models;

internal class Parameters
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
    public const int step = 3;
}