namespace ODataConsole.Framework;

public class UpdateStateResult
{
    public static UpdateStateResult Ok => new(UpdateStateStatus.Ok);
    public static UpdateStateResult NoChanges => new(UpdateStateStatus.NoChanges);
    public static UpdateStateResult Back => new(UpdateStateStatus.Back);
    public static UpdateStateResult Exit => new(UpdateStateStatus.Exit);

    public static UpdateStateResult OpenMenu(string menu) => new(UpdateStateStatus.ChangeScreen, menu);

    protected UpdateStateResult(UpdateStateStatus status)
    {
        Status = status;
    }

    protected UpdateStateResult(UpdateStateStatus status, string? data)
    {
        Status = status;
        Data = data;
    }

    public UpdateStateStatus Status { get; init; }
    public string? Data { get; init; }
}