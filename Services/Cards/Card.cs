namespace queensblood;

public abstract class Card
{
    public string Name { get; protected set; } = "";

    public bool Replaces { get; protected set; } = false;

    public int Cost { get; protected set; } = 0;

    public int Power { get; protected set; } = 0;

    public int RankBoost { get; protected set; } = 1;

    public bool HasAbility { get; protected set; } = false;

    private int[] rankBoosts = [];
    private RankOffset[] rankPositions = [];
    public RankOffset[] RankPositions
    {
        get => rankPositions; protected set
        {
            rankPositions = value;
            rankBoosts = rankPositions.Select((position) => 12 + (5 * position.Row) + position.Cell).ToArray();
        }
    }

    private int[] abilityBoosts = [];
    private RankOffset[] abilityPositions = [];
    public RankOffset[] AbilityPositions
    {
        get => abilityPositions; protected set
        {
            abilityPositions = value;
            abilityBoosts = abilityPositions.Select((position) => 12 + (5 * position.Row) + position.Cell).ToArray();
        }
    }

    public Card() { }

    public virtual void RegisterWatcher(CardWatcher watcher) { }

    public bool HasRankBoost(int index)
    {
        return rankBoosts.Contains(index);
    }

    public bool HasAbilityBoost(int index)
    {
        return abilityBoosts.Contains(index);
    }

    public virtual Ability Enfeeble(int amount)
    {
        Power = Math.Max(0, Power - amount);
        return Ability.None;
    }

    public virtual Ability Enhance(int amount)
    {
        Power += amount;
        return Ability.None;
    }

    public virtual Ability Play() { return Ability.None; }

    public virtual Ability Destroy() { return Ability.None; }

    public virtual Ability CardDestroyed(FieldCell cell) { return Ability.None; }

    public virtual Ability CardPlayed(FieldCell cell) { return Ability.None; }

    public virtual Ability LaneWon() { return Ability.None; }

    public virtual Ability GameEnd() { return Ability.None; }
}