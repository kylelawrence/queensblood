namespace queensblood;

public enum Effect
{
    None,
    Raise,
    Lower,
    Destroy,
}

public enum Target
{
    None,
    Self,
    Ally,
    Enemy,
    Both,
}

public enum Trigger
{
    None,
    Played,
    InPlay,
    AllyPlayed,
    AllyDestroyed,
    SelfDestroyed,
    EnemyPlayed,
    EnemyDestroyed,
    AnyDestroyed,
    Enhanced,
}

public record Ability(
    Target Source = Target.Self,
    Trigger Trigger = Trigger.Played,
    Target Target = Target.Ally,
    Effect Effect = Effect.Raise,
    int Value = 0,
    int Field = 0)
{
    public static readonly Ability None = new(Target.None, Trigger.None, Target.None, Effect.None, 0);
}