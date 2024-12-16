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

public enum TargetType
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

public record Ability(Effect Effect, TargetType TargetType, int Value = 0, ValueType ValueType = ValueType.Power, TargetType TargetTrigger = TargetType.Self)
{
    public static readonly Ability None = new(Effect.None, TargetType.None);
}