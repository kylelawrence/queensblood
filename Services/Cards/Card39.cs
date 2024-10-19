namespace queensblood;

public class Card39 : Card
{
    public Card39()
    {
        Name = "Cockatrice";
        Cost = 2;
        Power = 3;
        RankPositions = [new(1, 0)];
        AbilityPositions = [new(1, 0)];
    }

    public override Ability Play()
    {
        return new(Effect.Destroy, TargetType.Both);
    }
}