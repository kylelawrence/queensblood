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
}