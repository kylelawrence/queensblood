namespace queensblood;

public class FieldCell(Card? card, int pins, PlayerType owner)
{
    public static readonly FieldCell Empty = new(null, 0, PlayerType.Undecided);

    public Card? Card { get; set; } = card;
    public int Pins { get; set; } = pins;
    public PlayerType Owner { get; set; } = owner;

    public Ability Destroy()
    {
        if (Card == null) return Ability.None;

        var ability = Card.Destroyed;
        Card = null;
        return ability;
    }
}