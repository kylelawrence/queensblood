using System.Collections.ObjectModel;

namespace queensblood;

public static class Cards
{
    private static readonly Card[] cards =
        [
            new("Security Officer", 1, 1, 141440, Ability.None),
            new("Riot Trooper", 2, 3, 4333700, Ability.None),
            new("Grenadier", 2, 3, 0, new(Target.Self, Trigger.Played, Target.Enemy, Effect.Lower, 4, 16384)),
            new("J-Unit Sweeper", 2, 2, 393600, Ability.None),
            new("Queen Bee", 1, 1, 4194308, Ability.None),
            new("Toxirat", 2, 2, 393216, Ability.None),
            new("Levrikon", 1, 2, 139264, Ability.None),
            new("Grasslands Wolf", 1, 2, 8320, Ability.None),
            new("Mu", 2, 1, 8448, Ability.None),
            new("Mandragora", 1, 1, 139264, Ability.None),
            new("Elphadunk", 2, 4, 133248, Ability.None),
            new("Cactuar", 1, 1, 139264, Ability.None),
            new("Crystalline Crab", 1, 1, 10368, Ability.None),
            new("Quetzalcoatl", 2, 3, 4456708, Ability.None),
            new("Zu", 2, 2, 328000, Ability.None),
            new("Devil Rider", 2, 4, 35872, Ability.None),
            new("Screamer", 3, 1, 469440, Ability.None),
            new("Flan", 1, 2, 67648, Ability.None),
            new("Crawler", 1, 2, 196800, Ability.None)
        ];

    public static Card At(int index) 
    {
        if (index < 0 || index >= cards.Length) return Card.Null;
        return cards[index];
    }

    public static readonly int Count = cards.Length;

    public static readonly ReadOnlyCollection<Card> All = new(cards);
}