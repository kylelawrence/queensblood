using System.Collections.ObjectModel;

namespace queensblood;

public interface ICardsService
{
    ReadOnlyCollection<Card> Cards { get; }
}