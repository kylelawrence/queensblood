namespace queensblood;

public class Card12 : Card
{
    public Card12()
    {
        Name = "Cactuar";
        Cost = 1;
        Power = 1;
        RankPositions = [new(0, 1), new (1, 0)];
        AbilityPositions = [new(2, 1)];
    }

    public override Ability Play()
    {
        return new(Effect.Enhance, TargetType.Ally, 3, ValueType.Power);
    }

    public override Ability Destroy()
    {
        return new(Effect.Enfeeble, TargetType.Ally, 3, ValueType.Power);
    }
}