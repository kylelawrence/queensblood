namespace queensblood;

public class Card10 : Card
{
    public Card10()
    {
        Name = "Mandragora";
        Cost = 1;
        Power = 1;
        HasAbility = true;
        RankPositions = [new(0, 1), new(1, 0)];
    }

    public override Ability Play()
    {
        return new(Effect.AddCard, TargetType.Self, 1);
    }
}