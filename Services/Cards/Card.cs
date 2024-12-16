namespace queensblood;

public record RankOffset(int Row, int Cell);
public record ImageOffsets(int XOffset = 0, int YOffset = 0, float Scale = 1.0f);

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
    Ability? enhanced = null,
    Ability? enfeebled = null,
    Ability? power7 = null,
    int rankBoost = 1,
    bool legendary = false,
    string description = "This card has no abilities.",
    ImageOffsets? imageOffsets = null,
    ImageOffsets? fieldImageOffsets = null)
{
    public bool HasBeenEnfeebled { get; private set; } = false;
    public bool HasBeenEnhanced { get; private set; } = false;
    public bool hasHitPower7 = false;

    public readonly string Name = name;
    public readonly bool Replaces = cost == -1;
    public readonly int Cost = cost;
    public readonly string ImageUrl = $"/images/{name.ToLower()}.gif";
    public readonly ImageOffsets ImageOffsets = imageOffsets ?? new();
    public readonly ImageOffsets FieldImageOffsets = fieldImageOffsets ?? new();
    public int Power { get; private set; } = power;
    public readonly int RankBoost = rankBoost;
    public readonly bool HasAbility = inPlay != null || played != null || destroyed != null || cardDestroyed != null || cardPlayed != null || laneWon != null || enhanced != null || enfeebled != null || power7 != null;
    public readonly bool Legendary = legendary;
    public readonly string Description = description;

    public int PowerAdjustment { get; set; } = 0;
    public int SelfAdjustment { get; set; } = 0;

    public int AdjustedPower => Power + PowerAdjustment + SelfAdjustment;

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

    private readonly Ability power7 = power7 ?? Ability.None;
    public Ability Power7
    {
        get
        {
            if (hasHitPower7) return Ability.None;
            if (Power < 7) return Ability.None;
            hasHitPower7 = true;
            return power7;
        }
    }

    private readonly Ability enhanced = enhanced ?? Ability.None;
    public Ability Enhanced
    {
        get
        {
            if (HasBeenEnhanced) return Ability.None;
            HasBeenEnhanced = true;
            return enhanced;
        }
    }

    private readonly Ability enfeebled = enfeebled ?? Ability.None;
    public Ability Enfeebled
    {
        get
        {
            if (HasBeenEnfeebled) return Ability.None;
            HasBeenEnfeebled = true;
            return enfeebled;
        }
    }

    public bool HasRankBoost(int index) => rankBoosts.Contains(index);

    public bool HasAbilityBoost(int index) => abilityBoosts.Contains(index);

    public void Enfeeble(int amount)
    {
        Power = Math.Max(0, Power - amount);
    }

    public void Enhance(int amount)
    {
        Power += amount;
    }
}