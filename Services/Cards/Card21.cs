namespace queensblood;

public class Card21 : Card
{
    public Card21()
    {
        Name = "Ogre";
        Cost = 2;
        Power = 5;
        RankPositions = [new(-2, 0), new(-2, 1), new(2, 0), new(2, 1)];
    }
}