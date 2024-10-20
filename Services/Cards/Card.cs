namespace queensblood;

public class Card(
    string name,
    int cost,
    int power,
    RankOffset[]? rankPositions = null,
    RankOffset[]? abilityPositions = null,
    Ability? inPlay = null,
    Ability? played = null,
    Ability? destroyed = null,
    Ability? cardDestroyed = null,
    Ability? cardPlayed = null,
    Ability? laneWon = null,
    Ability? gameEnd = null,
    Ability? enhanced = null,
    Ability? enfeebled = null,
    Ability? power7 = null,
    int rankBoost = 1,
    bool legendary = false,
    string description = "This card has no abilities.")
{
    private bool hasBeenEnfeebled = false;
    private bool hasBeenEnhanced = false;

    public string Name => name;
    public readonly bool Replaces = cost == -1;
    public int Cost => cost;
    public int Power { get; private set; } = power;
    public int RankBoost => rankBoost;
    public readonly bool HasAbility = played != null || destroyed != null || cardDestroyed != null || cardPlayed != null || laneWon != null || gameEnd != null || enhanced != null || enfeebled != null;
    public readonly bool Legendary = legendary;

    private readonly int[] rankBoosts = (rankPositions ?? []).Select((position) => 12 + (5 * position.Row) + position.Cell).ToArray();
    public RankOffset[] RankPositions => rankPositions ?? [];

    private readonly int[] abilityBoosts = (abilityPositions ?? []).Select((position) => 12 + (5 * position.Row) + position.Cell).ToArray();
    public RankOffset[] AbilityPositions => abilityPositions ?? [];

    public Ability InPlay => inPlay ?? Ability.None;
    public Ability Played => played ?? Ability.None;
    public Ability Destroyed => destroyed ?? Ability.None;
    public Ability CardDestroyed => cardDestroyed ?? Ability.None;
    public Ability CardPlayed => cardPlayed ?? Ability.None;
    public Ability LaneWon => laneWon ?? Ability.None;
    public Ability GameEnd => gameEnd ?? Ability.None;
    public Ability Power7 => power7 ?? Ability.None;

    private readonly Ability enhanced = enhanced ?? Ability.None;
    public Ability Enhanced => hasBeenEnhanced ? Ability.None : enhanced;

    private readonly Ability enfeebled = enfeebled ?? Ability.None;
    public Ability Enfeebled => hasBeenEnfeebled ? Ability.None : enfeebled;

    public bool HasRankBoost(int index) => rankBoosts.Contains(index);

    public bool HasAbilityBoost(int index) => abilityBoosts.Contains(index);

    public void Enfeeble(int amount)
    {
        Power = Math.Max(0, Power - amount);
        hasBeenEnfeebled = true;
    }

    public void Enhance(int amount)
    {
        Power += amount;
        hasBeenEnhanced = true;
    }
}