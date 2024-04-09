using queensblood;
using queensblood.Components;

// using MongoDB.Driver;

// var dbConnect = Environment.GetEnvironmentVariable("COSMOS_CONNECTION_STRING");
// var dbClient = new MongoClient(dbConnect);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services
    // .AddSingleton(dbClient.GetDatabase("queensblood"))
    .AddSingleton<ICardsService, CardsStaticService>()
    .AddSingleton<IGamesService, GamesMongoService>()
    .AddScoped<IDecksService, DecksLocalService>()
    // .AddSingleton<ICardSetService, CardSetMongoService>() // Obselete
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

app.MapPlayRoutes();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
