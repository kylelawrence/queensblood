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
    let deck = index === 0 ? {
        cards: [
            { index: 0, count: 2 },
            { index: 1, count: 2 },
            { index: 2, count: 2 },
            { index: 3, count: 2 },
            { index: 4, count: 2 },
            { index: 5, count: 2 },
            { index: 6, count: 2 },
            { index: 7, count: 1 },
        ]
    } : { cards: [] };

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
