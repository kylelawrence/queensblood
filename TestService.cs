namespace queensblood;

public class TestService
{
    public int Derps { get; private set; }

    public event EventHandler Derp = delegate { };

    public void Do()
    {
        Derps++;
        Derp(this, EventArgs.Empty);
    }
}