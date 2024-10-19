namespace queensblood;

public class Card15 : Card
{
    public Card15()
    {
        Name = "Zu";
        Cost = 2;
        Power = 2;
        RankPositions = [new(-1, -1), new(-1, 1), new(1, -1), new(1, 1)];
    }
}