namespace ODataConsole.Framework;

public interface IAppScreen
{
    public string Key { get; }
    void Render();
    UpdateStateResult KeyPressed(ConsoleKeyInfo key);
}