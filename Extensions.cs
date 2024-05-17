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

    public static bool IsConnected(this CircuitHandler handler)
    {
        var asService = handler as CircuitService;
        return asService?.IsConnected ?? false;
    }
}