namespace queensblood;

public enum Effect
{
    None,
    Enfeeble,
    Enhance,
    Destroy,
    AddCard,
    AddScore,
    PartyAnimal,
    SpawnCards,
}

public enum CardRelation
{
    None,
    Ally,
    Enemy,
    Both,
    Self,
}

public enum ValueType
{
    Power,
    ReplacedPower,
    Enfeebled,
    Enhanced,
}

public record Ability(Effect Effect, CardRelation TargetType, int Value = 0, ValueType ValueType = ValueType.Power, CardRelation TargetTrigger = CardRelation.Self)
{
    public static readonly Ability None = new(Effect.None, CardRelation.None);
}