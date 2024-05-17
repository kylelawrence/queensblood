using System.Text.Json;
using Microsoft.JSInterop;

namespace queensblood;

/// <summary>
/// Saves and loads decks via JS calls to localStorage.
/// Must be added to services via AddScoped due to IJSRuntime dependency.
/// </summary>
/// <param name="js">JS Interop dependency</param>
public class DecksLocalService(IJSRuntime js) : IDecksService
{
    private const string GET_ITEM = "localStorage.getItem";
    private const string SET_ITEM = "localStorage.setItem";
    private const string SELECTED_DECK_KEY = "selected_deck";

    private readonly IJSRuntime js = js;

    public async Task SaveDeck(int index, DeckCard[] cards)
    {
        var json = JsonSerializer.Serialize(new Deck(cards));
        await js.InvokeVoidAsync(SET_ITEM, $"deck{index}", json).Try();
    }

    public async Task<Deck> LoadDeck(int index)
    {
        var response = await js.InvokeAsync<string>(GET_ITEM, $"deck{index}").Try();
        if (!response.Success || response.Value == null)
            return index == 0 ? Deck.BuildDefault() : Deck.None;

        var deck = JsonSerializer.Deserialize<Deck>(response.Value);
        await js.InvokeVoidAsync(SET_ITEM, SELECTED_DECK_KEY, index.ToString()).Try();

        return deck ?? Deck.BuildDefault();
    }

    public async Task<int> GetSelectedDeck()
    {
        var response = await js.InvokeAsync<string>(GET_ITEM, SELECTED_DECK_KEY).Try();
        if (!response.Success) return 0;
        return int.TryParse(response.Value, out var selectedDeck) ? selectedDeck : 0;
    }
}