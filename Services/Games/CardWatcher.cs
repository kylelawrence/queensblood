namespace queensblood;

public class CardWatcher()
{
    public event EventHandler<FieldCell> OnCardPlayed = delegate { };

    public event EventHandler<FieldCell> OnCardDestroyed = delegate { };

    public event EventHandler<FieldCell> OnGameEnd = delegate { };

    public void CardPlayed(FieldCell cell)
    {
        OnCardPlayed(this, cell);
    }

    public void CardDestroyed(FieldCell cell)
    {
        OnCardDestroyed(this, cell);
    }

    public void GameEnd(FieldCell cell)
    {
        OnGameEnd(this, cell);
    }
}