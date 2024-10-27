using Microsoft.OData.SampleService.Models.TripPin;
using ODataConsole.Framework;
using ODataConsole.Infrastructure;
using Spectre.Console;

namespace ODataConsole.Screens;

public class DetailsScreen(IPeopleService peopleService) : IAppScreen
{
    private string InputUserName => String.Join("", _input.Reverse());
    private readonly Stack<char> _input = new();

    private record FetchResult(Person? Person, string? Error);
    
    private FetchResult? _fetched;

    public const string ScreenKey = nameof(DetailsScreen);
    public string Key => ScreenKey;

    public void Render()
    {
        if (_fetched?.Person != null)
        {
            AnsiConsole.Write(
                new Panel(
                    new Table()
                        .AddColumn("Property")
                        .AddColumn("Value")
                        .AddRow("User Name", _fetched.Person.UserName)
                        .AddRow("Last Name", _fetched.Person.LastName)
                        .AddRow("Emails", String.Join(",", _fetched.Person.Emails))));
            _fetched = null;
            return;
        }
        
        AnsiConsole.Clear();
        var message = @$"
[white]Enter query:[/]
{InputUserName}[bold yellow]_[/]

Press enter to search.
[red]{_fetched?.Error}[/]

To return to previous menu, press Escape.";
        
        AnsiConsole.Write(Align.Left(new Markup(message)));
    }

    public UpdateStateResult KeyPressed(ConsoleKeyInfo key)
    {
        if (key.Key == ConsoleKey.Spacebar || key.IsLetter() || key.IsNumber())
        {
            var ch = key.IsShiftPressed() ? Char.ToUpper(key.KeyChar) : key.KeyChar;
            _input.Push(ch);
            return UpdateStateResult.Ok;
        }
        if (key.Key == ConsoleKey.Enter)
        {
            if (String.IsNullOrEmpty(InputUserName))
            {
                _fetched = new FetchResult(null, "Please enter valid username.");
                return UpdateStateResult.Ok;
            }

            var person = peopleService.GetDetailsAsync(InputUserName);
            if (person is null)
            {
                _fetched = new FetchResult(null, "No person found with the given user name.");
            }
            else
            {
                _fetched = new FetchResult(person, null);
                _input.Clear();
            }

            return UpdateStateResult.Ok;
        }

        if (key.Key == ConsoleKey.Backspace && _input.Count > 0)
        {
            _input.Pop();
            return UpdateStateResult.Ok;
        }

        if (key.Key == ConsoleKey.Escape)
        {
            return UpdateStateResult.Back;
        }

        return UpdateStateResult.NoChanges;
    }
}