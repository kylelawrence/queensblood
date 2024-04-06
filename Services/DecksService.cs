using System.Collections.ObjectModel;

namespace queensblood;

public class Deck
{
    public string Name { get; set; } = "";
    public int CardSet { get; set; } = 1;
    public List<char> Cards { get; set; } = [];

    public override string ToString()
    {
        return "";
    }
}

public interface IDecksService
{

}

public class DecksCookieService : IDecksService
{
    private const string COOKIE_NAME = "decks";

    private IHttpContextAccessor contextAccessor;

    public DecksCookieService(IHttpContextAccessor context)
    {
        contextAccessor = context;
    }

    public ReadOnlyCollection<Deck> GetDecks()
    {
        return new([new(), new(), new(), new(), new()]);
    }

    public void SaveDecks()
    {

    }
}