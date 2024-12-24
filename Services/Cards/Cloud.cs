namespace queensblood;

public class Cloud : Card
{
    public Cloud() :
        base("Cloud", 2, 3,
            [new(0, 1)],
            [new(-1, -1), new(-1, 0), new(-1, 1), new(0, -1), new(0, 1), new(1, -1), new(1, 0), new(1, 1)],
            enhanced: new(Effect.Enhance, CardRelation.Ally, 2),
            legendary: true,
            description: "When this card's power first reaches 7, raise the power of allied cards on affected tiles by 2.")
    {
    }

    private bool hasHitPower7 = false;
    new public Ability Enhanced
    {
        get
        {
            if (hasHitPower7) return Ability.None;
            if (Power >= 7)
            {
                HasBeenEnhanced = true;
                hasHitPower7 = true;
                return enhanced;
            }
            
            if (HasBeenEnhanced) return Ability.None;
            HasBeenEnhanced = true;
            return Ability.None;
        }
    }
}