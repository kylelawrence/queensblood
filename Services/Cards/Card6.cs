namespace queensblood;

public class Card6 : Card
{
    public Card6()
    {
        Name = "Toxirat";
        Cost = 2;
        Power = 2;
        HasAbility = true;
        RankPositions = [new(0, 1), new(1, 1)];
        AbilityPositions = [new(1, 1)];
    }

    public override Ability Play()
    {
        return new(Effect.Enfeeble, TargetType.Both, 3, ValueType.Power);
    }
}