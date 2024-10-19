namespace queensblood;

public class Card3 : Card
{
    public Card3()
    {
        Name = "Grenadier";
        Cost = 2;
        Power = 1;
        HasAbility = true;
        AbilityPositions = [new(2, 0)];
    }

    public override Ability Play()
    {
        return new(Effect.Enfeeble, TargetType.Enemy, 4, ValueType.Power);
    }
}