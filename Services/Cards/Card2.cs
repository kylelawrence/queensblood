namespace queensblood;

public class Card2 : Card
{
    public Card2()
    {
        Name = "Riot Trooper";
        Cost = 2;
        Power = 3;
        RankPositions = [new(0, -2), new(0, -1), new(0, 1), new(0, 2), new(1, 0)];
    }
}