using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OData.SampleService.Models.TripPin;

namespace ODataConsole.Infrastructure;

public interface IPeopleService
{
    CursorResult<IEnumerable<Person>> List(int count, Cursor? cursor = null);
    CursorResult<IEnumerable<Person>> SearchByName(string searchTerm, int count, Cursor? cursor = null);
    Person? GetDetailsAsync(string userName);
}

public class PeopleService : IPeopleService
{
    private readonly DefaultContainer _container;

    public PeopleService(ILogger<PeopleService> logger, IOptions<ODataSettings> options)
    {
        var settings = options.Value;
        _container = new DefaultContainer(new Uri(settings.Url));
    }

    public CursorResult<IEnumerable<Person>> List(int count, Cursor? cursor = null)
    {
        var offset = cursor?.Offset ?? 0;
        var people = _container.People
            .OrderBy(p => p.UserName)
            .Skip(offset)
            .Take(count)
            .AsEnumerable();

        var nextCursor = new Cursor(offset + count);
        return new CursorResult<IEnumerable<Person>>(people, nextCursor);
    }

    public CursorResult<IEnumerable<Person>> SearchByName(string searchTerm, int count, Cursor? cursor = null)
    {
        var offset = cursor?.Offset ?? 0;
        var people = _container.People
            .Where(p => p.FirstName.Contains(searchTerm) || p.LastName.Contains(searchTerm))
            .OrderBy(p => p.UserName)
            .Skip(offset)
            .Take(count)
            .AsEnumerable();
        
        var nextCursor = new Cursor(offset + count);
        return new CursorResult<IEnumerable<Person>>(people, nextCursor);
    }

    public Person? GetDetailsAsync(string userName)
    {
        return _container.People.SingleOrDefault(p => p.UserName == userName);
    }
}