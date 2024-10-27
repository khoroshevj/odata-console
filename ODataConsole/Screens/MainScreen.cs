using ODataConsole.Framework;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ODataConsole.Screens;

public class MainScreen : IMainScreen
{
    private const string MenuSlot = "Menu";
    private const string CommandDescriptionSlot = "Description";

    private readonly Layout _layout = new Layout("Root")
        .SplitColumns(
            new Layout(MenuSlot).Size(64),
            new Layout(CommandDescriptionSlot).Size(64));

    private record Choice(string Name, string Description, UpdateStateResult UpdateState);

    private readonly Choice[] _choices =
    {
        new(
            "Search people",
            "Search for people by first name or last name",
            UpdateStateResult.OpenMenu(SearchScreen.ScreenKey)),

        new(
            "Show person", 
            "Show details of a specific person",
            UpdateStateResult.OpenMenu(DetailsScreen.ScreenKey)),
  
        new("Exit", "Exit the application", UpdateStateResult.Exit)
    };

    private int _state = 0;

    public const string ScreenKey = nameof(MainScreen);
    public string Key => ScreenKey;

    public void Render()
    {
        _layout[MenuSlot].Update(
            new Panel(RenderChoices(_choices, _state)).Expand());

        _layout[CommandDescriptionSlot].Update(
            new Panel(RenderChoiceDescription(_choices[_state])) { Border = BoxBorder.Ascii }.Expand());

        AnsiConsole.Write(_layout);
    }

    private static IRenderable RenderChoices(Choice[] choices, int state)
    {
        var list = String.Join("\n", choices.Select((c, i) => i == state ? $"[blue]{c.Name}[/]" : c.Name));
        return Align.Center(
            new Markup("[green]Choose an option:[/]\n\n" + list),
            VerticalAlignment.Middle);
    }

    private static IRenderable RenderChoiceDescription(Choice choice)
    {
        return Align.Center(
            new Markup(choice.Description),
            VerticalAlignment.Top);
    }

    public UpdateStateResult KeyPressed(ConsoleKeyInfo key)
    {
        switch (key.Key)
        {
            case ConsoleKey.PageUp:
            case ConsoleKey.UpArrow:
                _state = Math.Max(0, _state - 1);
                return UpdateStateResult.Ok;

            case ConsoleKey.PageDown:
            case ConsoleKey.Tab:
            case ConsoleKey.DownArrow:
                _state = Math.Min(_choices.Length - 1, _state + 1);
                return UpdateStateResult.Ok;

            case ConsoleKey.Enter:
                return _choices[_state].UpdateState;
                
            case ConsoleKey.None:
            default:
                return UpdateStateResult.NoChanges;
        }
    }
}