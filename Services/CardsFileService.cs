namespace queensblood;

public interface ICardsFileService
{
    public string[] ReadCardsFile();
    public Task<bool> SaveCardsFile(string[] lines);
}

public class CardsFileService : ICardsFileService
{
    public const string ALL_CARDS_FILES = "wwwroot/cards.tsv";

    public static readonly CardsFileService Instance = new();

    private CardsFileService()
    { }

    public string[] ReadCardsFile()
    {
        try
        {
            return File.ReadAllLines(ALL_CARDS_FILES);
        }
        catch
        {
            return [];
        }
    }

    public async Task<bool> SaveCardsFile(string[] lines)
    {
        try
        {
            await File.WriteAllLinesAsync(ALL_CARDS_FILES, lines);
            return true;
        }
        catch
        {
            return false;
        }
    }
}