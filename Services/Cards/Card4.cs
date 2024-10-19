namespace queensblood;

public class Card4 : Card
{
    public Card4()
    {
        Name = "J-Unit Sweeper";
        Cost = 2;
        Power = 2;
        RankPositions = [new(0, -1), new(1, -1), new(0, 1), new(1, 1)];
    }
}