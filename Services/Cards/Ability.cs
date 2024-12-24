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
    EnhancedOrEnfeebled,
}

public record Ability(Effect Effect, CardRelation CardTypeToTarget, int Value = 0, ValueType ValueType = ValueType.Power, CardRelation TriggeringCardType = CardRelation.Self)
{
    public static readonly Ability None = new(Effect.None, CardRelation.None);
}