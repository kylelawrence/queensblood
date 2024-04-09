const SELECTED_DECK_KEY = 'selected_deck';

function saveDeck(index, deck) {
    try {
        const key = `deck${index}`;
        const value = JSON.stringify(deck);
        localStorage.setItem(key, value);
    } catch { }
}

function loadDeck(index) {
    const key = `deck${index}`;
    let deck = { name: key, cards: [] };

    try {
        const value = localStorage.getItem(key);
        if (value) deck = JSON.parse(value);
    } catch { }

    localStorage.setItem(SELECTED_DECK_KEY, index);
    return deck;
}

function getSelectedDeck() {
    return localStorage.getItem(SELECTED_DECK_KEY) ?? "0";
}
