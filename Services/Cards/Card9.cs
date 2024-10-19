namespace queensblood;

public class Card9 : Card
{
    public Card9()
    {
        Name = "Mu";
        Cost = 2;
        Power = 1;
        HasAbility = true;
        RankPositions = [new(0, 1), new(-1, 1)];
        AbilityPositions = [new(1, 0)];
    }

    public override Ability Play()
    {
        return new(Effect.Enhance, TargetType.Both, 1, ValueType.Power);
    }

    public override Ability Destroy()
    {
        return new(Effect.Enfeeble, TargetType.Both, 1, ValueType.Power);
    }
}