using Microsoft.Extensions.Logging.Testing;
using Microsoft.Extensions.Options;
using ODataConsole.Infrastructure;

namespace ODataConsole.Tests;

public class PeopleServiceTests
{
    [Fact]
    public void List_Ok()
    {
        var peopleService = new PeopleService(new FakeLogger<PeopleService>(), Options.Create(new ODataSettings()));

        var result = peopleService.List(5);

        Assert.NotNull(result.Cursor);
        var resultPeople = result.Result.ToList();
        Assert.NotEmpty(resultPeople);
        Assert.Equal(5, resultPeople.Count());
        
        var nextResult = peopleService.List(5, result.Cursor);
        
        Assert.NotNull(result.Cursor);
        var nextResultPeople = nextResult.Result.ToList();
        Assert.NotEmpty(nextResultPeople);
        Assert.Equal(5, nextResultPeople.Count);

        Assert.All(
            resultPeople,
            person => Assert.DoesNotContain(nextResultPeople, person2 => person2.UserName == person.UserName));
    }
}