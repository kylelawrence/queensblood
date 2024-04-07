const SELECTED_DECK_KEY = 'selected_deck';

function saveDeck(index, deck) {
    try {
        const key = `deck${index}`;
        const value = JSON.stringify(deck);
        localStorage.setItem(key, value);
        return true;
    } catch {
        return false;
    }
}

function loadDeck(index) {
    try {
        const key = `deck${index}`;
        const value = localStorage.getItem(key);
        if (!value) {
            localStorage.setItem(SELECTED_DECK_KEY, index);
            return {
                name: key,
                iteration: -1,
                cards: []
            };
        }
        const deck = JSON.parse(value);

        localStorage.setItem(SELECTED_DECK_KEY, index);
        return deck;
    } catch {
        return {
            name: "!!ERROR!!",
            iteration: -1,
            cards: []
        };
    }
}

function getSelectedDeck() {
    return localStorage.getItem(SELECTED_DECK_KEY);
}
