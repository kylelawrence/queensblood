using Microsoft.JSInterop;

namespace queensblood;

/// <summary>
/// Saves and loads decks via JS calls to localStorage.
/// Must be added to services via AddScoped due to IJSRuntime dependency.
/// </summary>
/// <param name="js">JS Interop dependency</param>
public class DecksLocalService(IJSRuntime js) : IDecksService
{
    private const string SAVE_DECK_FN = "saveDeck";
    private const string LOAD_DECK_FN = "loadDeck";
    private const string SELECTED_DECK_FN = "getSelectedDeck";

    private readonly IJSRuntime js = js;

    public async Task SaveDeck(int index, DeckCard[] cards)
    {
        var deck = new Deck(cards);
        await js.InvokeVoidAsync(SAVE_DECK_FN, index, deck).Try();
    }

    public async Task<Deck> LoadDeck(int index)
    {
        var result = await js.InvokeAsync<Deck>(LOAD_DECK_FN, index).Try();
        return result.Success ? result.Value : new([]);
    }

    public async Task<int> GetSelectedDeck()
    {
        var result = await js.InvokeAsync<string>(SELECTED_DECK_FN).Try();
        if (!result.Success) return 0;
        return int.TryParse(result.Value, out var selectedDeck) ? selectedDeck : 0;
    }
}