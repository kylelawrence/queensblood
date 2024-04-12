public static class BaseHandlers
{
    public static void GoHome(HttpContext context) => context.Response.Redirect("/");
}