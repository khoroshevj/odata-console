using Microsoft.Extensions.Hosting;
using ODataConsole.Framework;
using ODataConsole.Screens;
using Spectre.Console;

namespace ODataConsole;

public class App : IHostedService
{
    private CancellationTokenSource _cancellationToken;
    private readonly Dictionary<string, IAppScreen> _screens;
    private readonly IAppScreen _mainScreen;

    public App(IServiceProvider serviceProvider, IEnumerable<IAppScreen> screens)
    {
        _cancellationToken = new CancellationTokenSource();
        _screens = screens.ToDictionary(screen => screen.Key, screen => screen);
        _mainScreen = _screens.FirstOrDefault(screen => screen.Key == MainScreen.ScreenKey).Value;
        if (_mainScreen == null)
        {
            throw new InvalidOperationException("No main screen found.");
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var stack = new Stack<IAppScreen>();
        stack.Push(_mainScreen);

        while (stack.Count > 0 && !Cancelling())
        {
            var currentMenu = stack.Peek();
            currentMenu.Render();

            UpdateStateResult? updateResult;

            do
            {
                var keyPressed = Console.ReadKey();
                updateResult = currentMenu.KeyPressed(keyPressed);
            } while (updateResult.Status == UpdateStateStatus.NoChanges && !Cancelling());

            switch (updateResult.Status)
            {
                case UpdateStateStatus.Ok:
                    break;

                case UpdateStateStatus.ChangeScreen:
                    stack.Push(_screens[updateResult.Data!]);
                    break;

                case UpdateStateStatus.Back:
                    stack.Pop();
                    break;

                case UpdateStateStatus.Exit:
                    stack.Clear();
                    break;
            }
        }

        AnsiConsole.Console.Clear();
        AnsiConsole.Console.WriteLine("Buy \ud83d\udc4b");
        
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _cancellationToken.CancelAsync();
    }

    private bool Cancelling()
    {
        return _cancellationToken.IsCancellationRequested == true;
    }
}