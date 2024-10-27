using Microsoft.OData.SampleService.Models.TripPin;
using ODataConsole.Framework;
using ODataConsole.Infrastructure;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ODataConsole.Screens;

public class SearchScreen(IPeopleService peopleService) : IAppScreen
{
    private readonly Layout _layout = new Layout("Root")
        .SplitColumns(
            new Layout(InputSlot),
            new Layout(ResultsSlot));
    
    private static IRenderable Loading =  Align.Center(new Markup($"[white]Loading...[/]."));

    private const string InputSlot = "Input";
    private const string ResultsSlot = "Results";

    private string SearchQuery => String.Join("", _input.Reverse());
    private readonly Stack<char> _input = new();

    private CursorResult<IEnumerable<Person>>? _search;
    private bool _searchRendered = false;
    private readonly Stack<CursorResult<IEnumerable<Person>>> _searchPages = new();

    public const string ScreenKey = nameof(SearchScreen);
    public string Key => ScreenKey;

    public void Render()
    {
        _layout[InputSlot].Update(new Panel(RenderInput(SearchQuery)).Expand());
        if (_search != null && !_searchRendered)
        {
            _layout[ResultsSlot].Update(Loading);
            AnsiConsole.Write(_layout);
            
            _layout[ResultsSlot].Update(new Panel(RenderSearchResults(_search, _searchPages.Count + 1)).Expand());
            _searchRendered = true;
        }

        AnsiConsole.Write(_layout);
    }

    private static IRenderable RenderInput(string input)
    {
        return Align.Left(
            new Markup($"[white]Enter query:[/]\n{input}[bold yellow]_[/]\nPress enter to search.\n\nTo return to previous menu, press Escape."));
    }

    private static IRenderable RenderSearchResults(CursorResult<IEnumerable<Person>> searchResult, int pageNumber)
    {
        var names = searchResult
            .Result
            .Select(p => new { p.UserName, p.LastName })
            .ToList();

        var resultList = String.Join("\n",names.Select(p => $"[bold white]{p.UserName} {p.LastName}[/]"));

        return Align.Left(
            new Markup($"Results page {pageNumber}:\n{resultList}\n\nUse left/right arrows to navigate between pages."),
            VerticalAlignment.Top);
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
            _search = peopleService.SearchByName(SearchQuery, 10);
            _searchRendered = false;
            return UpdateStateResult.Ok;
        }

        if (key.Key == ConsoleKey.RightArrow && _search != null)
        {
            var pageResult = peopleService.SearchByName(SearchQuery, 10, _search.Cursor);
            if (pageResult.Result.Any())
            {
                _searchPages.Push(_search);
                _search = pageResult;
            }

            _searchRendered = false;
            return UpdateStateResult.Ok;
        }

        if (key.Key == ConsoleKey.LeftArrow && _searchPages.Count > 0)
        {
            _search = _searchPages.Pop();
            _searchRendered = false;
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