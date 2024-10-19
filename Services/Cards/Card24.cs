namespace queensblood;

public class Card24 : Card
{
    public Card24()
    {
        Name = "Zemzelett";
        Cost = 2;
        Power = 1;
        RankPositions = [new(-1, 0), new(1, 0), new(1, 1)];
        AbilityPositions = [new(0, -1)];
        HasAbility = true;
    }

    public override Ability Play()
    {
        return new(Effect.Enfeeble, TargetType.Both, 3, ValueType.Power);
    }
}