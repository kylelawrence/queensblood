using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace queensblood;

public record TryResult<T>(bool Success, T Value);

public static class Extensions
{
    public static async Task<bool> Try(this Func<Task> func)
    {
        try { await func().ConfigureAwait(false); }
        catch { return false; }
        return true;
    }

    public static async Task<bool> Try(this Task task)
    {
        try { await task.ConfigureAwait(false); }
        catch { return false; }
        return true;
    }

    public static async ValueTask<TryResult<T>> Try<T>(this ValueTask<T> task)
    {
        try { return new(true, await task.ConfigureAwait(false)); }
        catch { return new(false, default!); }
    }

    public static async ValueTask<bool> Try(this ValueTask task)
    {
        try { await task.ConfigureAwait(false); }
        catch { return false; }
        return true;
    }

    public static string MaybeClass(this ComponentBase component, string className, bool check)
    {
        return check ? className : "";
    }

    public static bool TryGetCookie(this HttpContext context, string cookieName, out string? value)
    {
        return context.Request.Cookies.TryGetValue(cookieName, out value);
    }

    public static void SetCookie(this HttpContext context, string cookieName, string value)
    {
        context.Response.Cookies.Append(cookieName, value);
    }

    private static readonly int[] fieldValues =
        [1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, 131072, 262144, 524288, 1048576, 2097152, 4194304, 8388608, 16777216];

    public static bool FieldIsBoosted(this Card card, int index)
    {
        if (index < 0 || index > 24 || index == 12) return false;
        return (card.Boosts & fieldValues[index]) != 0;
    }

    public static Card ToggleBoost(this Card card, int index)
    {
        if (index < 0 || index > 24 || index == 12) return card;
        var newBoosts = card.Boosts ^ fieldValues[index];
        return new(card.Name, card.PinCost, card.Value, newBoosts);
    }

    public static bool IsConnected(this CircuitHandler handler)
    {
        var asService = handler as CircuitService;
        return asService?.IsConnected ?? false;
    }
}