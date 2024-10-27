namespace ODataConsole.Framework;

public static class ConsoleKeyInfoExtensions
{
    public static bool IsShiftPressed(this ConsoleKeyInfo key)
    {
        return (key.Modifiers & ConsoleModifiers.Shift)!= 0;
    }

    public static bool IsLetter(this ConsoleKeyInfo key)
    {
        return key.Key is >= ConsoleKey.A and <= ConsoleKey.Z;
    }

    public static bool IsNumber(this ConsoleKeyInfo consoleKeyInfo)
    {
        return ConsoleKey.D0 <= consoleKeyInfo.Key && consoleKeyInfo.Key <= ConsoleKey.D9;
    }
}