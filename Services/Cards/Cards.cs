using System.Collections.ObjectModel;

namespace queensblood;

public static class Cards
{
    private static readonly Card[] cards =
        [
            new("Security Officer", 1, 1, 141440),
            new("Riot Trooper", 2, 3, 4333700),
            new("J-Unit Sweeper", 2, 2, 393600),
            new("Queen Bee", 1, 1, 4194308),
            new("Toxirat", 2, 2, 393216),
            new("Levrikon", 1, 2, 139264),
            new("Grasslands Wolf", 1, 2, 8320),
            new("Mu", 2, 1, 8448),
            new("Mandragora", 1, 1, 139264),
            new("Elphadunk", 2, 4, 133248),
            new("Cactuar", 1, 1, 139264),
            new("Crystalline Crab", 1, 1, 10368),
            new("Quetzalcoatl", 2, 3, 4456708),
            new("Zu", 2, 2, 328000),
            new("Devil Rider", 2, 4, 35872),
            new("Screamer", 3, 1, 469440),
            new("Flan", 1, 2, 67648),
            new("Crawler", 1, 2, 196800)
        ];

    public static Card At(int index) 
    {
        if (index < 0 || index >= cards.Length) return Card.Null;
        return cards[index];
    }

    public static readonly int Count = cards.Length;

    public static readonly ReadOnlyCollection<Card> All = new(cards);
}