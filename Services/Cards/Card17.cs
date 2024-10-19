namespace queensblood;

public class Card17 : Card
{
    public Card17()
    {
        Name = "Screamer";
        Cost = 3;
        Power = 1;
        RankPositions = [new(-1, -1), new(-1, 0), new(-1, 1), new(0, -1), new(0, 1), new(1, -1), new(1, 0), new(1, 1)];
    }
}