namespace queensblood;

public class Card1 : Card
{
    public Card1()
    {
        Name = "Security Officer";
        Cost = 1;
        Power = 1;
        RankPositions = [new(-1, 0), new(1, 0), new(0, -1), new(0, 1)];
    }
}