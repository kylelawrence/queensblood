namespace queensblood;

public static class Boosts
{
    private static readonly int[] indexRowOffsets = [-2, -2, -2, -2, -2, -1, -1, -1, -1, -1, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2];
    private static readonly int[] indexColumnOffsets = [-2, -1, 0, 1, 2, -2, -1, 0, 1, 2, -2, -1, 0, 1, 2, -2, -1, 0, 1, 2, -2, -1, 0, 1, 2];

    public static int GetRowOffset(int index) => indexRowOffsets[index];
    public static int GetColumnOffset(int index) => indexColumnOffsets[index];
}