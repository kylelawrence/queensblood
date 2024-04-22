using Microsoft.AspNetCore.Components.Server.Circuits;

namespace queensblood;

public class CircuitService : CircuitHandler
{
    private Circuit? Circuit;

    public bool IsConnected => Circuit != null;

    public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        Circuit = circuit;
        return base.OnCircuitOpenedAsync(circuit, cancellationToken);
    }

    public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        if (Circuit == circuit) Circuit = null;
        return base.OnCircuitClosedAsync(circuit, cancellationToken);
    }

    public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        Circuit = circuit;
        return base.OnConnectionUpAsync(circuit, cancellationToken);
    }

    public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        if (Circuit == circuit) Circuit = null;
        return base.OnConnectionDownAsync(circuit, cancellationToken);
    }
}