namespace Friends.Game.Hexmaps;

/// <summary>
/// Position in hex coordinates.
/// Uses doubled coordinates
/// </summary>
struct DoubledCoord
{
    public DoubledCoord(int col, int row)
    {
        this.col = col;
        this.row = row;
    }
    public readonly int col;
    public readonly int row;

    static public DoubledCoord QdoubledFromCube(Hex h)
    {
        int col = h.Q;
        int row = 2 * h.R + h.Q;
        return new DoubledCoord(col, row);
    }


    public Hex QdoubledToCube()
    {
        int q = col;
        int r = (int)((row - col) / 2);
        int s = -q - r;
        return new Hex(q, r, s);
    }


    static public DoubledCoord RdoubledFromCube(Hex h)
    {
        int col = 2 * h.Q + h.R;
        int row = h.R;
        return new DoubledCoord(col, row);
    }


    public Hex RdoubledToCube()
    {
        int q = (int)((col - row) / 2);
        int r = row;
        int s = -q - r;
        return new Hex(q, r, s);
    }

}