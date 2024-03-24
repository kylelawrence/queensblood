using Microsoft.AspNetCore.Components;
using queensblood.Components.Parts;

namespace queensblood;

public static class Extensions
{
    public static async Task<bool> Try(this Func<Task> func)
    {
        try
        {
            await func().ConfigureAwait(false);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static async Task<bool> Try(this Task task)
    {
        try
        {
            await task.ConfigureAwait(false);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static string MaybeClass(this ComponentBase component, string className, bool check)
    {
        return check ? className : "";
    }

    private static readonly int[] fieldValues = [1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072, 262144, 524288, 1048576, 2097152, 4194304, 8388608, 16777216];
    public static bool FieldIsBoosted(this Card card, int index)
    {
        if (index < 0 || index > 24 || index == 12) return false;
        return (card.Boosts & fieldValues[index]) != 0;
    }
    public static Card ToggleBoost(this Card card, int index)
    {
        if (index < 0 || index > 24 || index == 12) return card;
        var newBoosts = card.Boosts ^ fieldValues[index];
        return new(
            card.Id,
            card.Name,
            card.PinCost,
            card.Value,
            newBoosts
        );
    }
}