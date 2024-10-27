namespace ODataConsole.Infrastructure;

public record CursorResult<TResult>(TResult Result, Cursor Cursor);