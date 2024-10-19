namespace queensblood;

public class Card22 : Card
{
    public Card22()
    {
        Name = "Deathwheel";
        Cost = 1;
        Power = 1;
        AbilityPositions = [new(-2, 2), new(2, 2)];
        HasAbility = true;
    }

    public override Ability Play()
    {
        return new(Effect.Enfeeble, TargetType.Both, 3, ValueType.Power);
    }
}