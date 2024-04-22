using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace queensblood;

public class GameWatcher : ComponentBase, IDisposable
{
    [Parameter] public Game Game { get; set; } = Game.None;
    [Inject] private CircuitHandler CircuitHandler { get; set; } = null!;

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            Game.OnGameUpdated -= HandleGameUpdated;
            Game.OnGameUpdated += HandleGameUpdated;
        }
    }

    public void Dispose()
    {
        Game.OnGameUpdated -= HandleGameUpdated;
        GC.SuppressFinalize(this);
    }

    private void HandleGameUpdated(Object? sender, EventArgs e)
    {
        if (!CircuitHandler.IsConnected())
        {
            Game.OnGameUpdated -= HandleGameUpdated;
            return;
        }

        this.InvokeAsync(StateHasChanged);
    }
}