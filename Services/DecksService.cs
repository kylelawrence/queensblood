using Microsoft.JSInterop;

namespace queensblood;

public record DeckCard(int index, int count);

public record Deck(string name, DeckCard[] cards);

public interface IDecksService
{
    Task SaveDeck(int index, string name, DeckCard[] cards);
    Task<Deck> LoadDeck(int index);
    Task<int> GetSelectedDeck();
}

public class DecksLocalService(IJSRuntime js) : IDecksService
{
    private const string SAVE_DECK_FN = "saveDeck";
    private const string LOAD_DECK_FN = "loadDeck";
    private const string SELECTED_DECK_FN = "getSelectedDeck";

    private readonly IJSRuntime js = js;

    public async Task SaveDeck(int index, string name, DeckCard[] cards)
    {
        var deck = new Deck(name, cards);
        await js.InvokeVoidAsync(SAVE_DECK_FN, index, deck).Try();
    }

    public async Task<Deck> LoadDeck(int index)
    {
        var result = await js.InvokeAsync<Deck>(LOAD_DECK_FN, index).Try();
        return result.Success ? result.Value : new($"deck{index}", []);
    }

    public async Task<int> GetSelectedDeck()
    {
        var selectedDeck = 0;
        var result = await js.InvokeAsync<string>(SELECTED_DECK_FN).Try();
        if (!result.Success) return selectedDeck;
        int.TryParse(result.Value, out selectedDeck);
        return selectedDeck;
    }
}