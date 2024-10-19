namespace queensblood;

public class Card20 : Card
{
    public Card20()
    {
        Name = "Archdragon";
        Cost = 1;
        Power = 3;
        RankPositions = [new(-1, -1), new(0, 1), new(1, -1)];
        AbilityPositions = [new(0, 1)];
        HasAbility = true;
    }

    public override Ability Play()
    {
        return new(Effect.Enfeeble, TargetType.Enemy, 3, ValueType.Power);
    }

    public override Ability Destroy()
    {
        return new(Effect.Enhance, TargetType.Enemy, 3, ValueType.Power);
    }
}