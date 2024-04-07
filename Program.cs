using queensblood;
using queensblood.Components;

using MongoDB.Driver;

const string DECK_COOKIE_NAME = "decks";

var dbConnect = Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING");
var dbClient = new MongoClient(dbConnect);
var db = dbClient.GetDatabase("queensblood");

var builder = WebApplication.CreateBuilder(args);
var cardsService = new CardsMongoService(db);

// Add services to the container.
builder.Services
    .AddSingleton<ICardsService>(cardsService)
    .AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// Refresh the decks cookie
app.Use((context, next) =>
{
    var newExpiry = new CookieOptions() { Expires = new DateTime(9999, 1, 1) };

    if (!context.Request.Cookies.ContainsKey(DECK_COOKIE_NAME))
    {
        context.Response.Cookies.Append(DECK_COOKIE_NAME, "", newExpiry);
    }
    else
    {
        var decks = context.Request.Cookies[DECK_COOKIE_NAME];
        context.Response.Cookies.Append(DECK_COOKIE_NAME, decks ?? "", newExpiry);
    }

    return next(context);
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
