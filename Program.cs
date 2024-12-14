using Microsoft.AspNetCore.Components.Server.Circuits;
using queensblood;
using queensblood.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    .AddHttpContextAccessor()
    .AddSingleton<IGamesService, GamesMemService>()
    .AddSingleton<IPlayersService, PlayersMemService>()
    // Local Decks depends on IJSRuntime, so it gets scoped to user
    .AddScoped<IDecksService, DecksLocalService>()
    .AddScoped<IPlayerIdAccessor, PlayerIdAccessor>()
    .AddScoped<CircuitHandler, CircuitService>()
    .AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change 
    //this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Ensure player has an ID
app.UsePlayerID();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/descs", () => {
    var descs = Cards.All.Where(c => c.HasAbility).Select(c => c.Description).OrderBy(d => d).Distinct().ToArray();
    return Results.Text(string.Join("\n", descs));
});

app.Run();
