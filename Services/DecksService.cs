using System.Collections.ObjectModel;

namespace queensblood;

public record DeckCard(int index, int count);

public record Deck(string name, int iteration, DeckCard[] cards);
