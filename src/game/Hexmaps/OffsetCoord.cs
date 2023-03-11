using System;

namespace Friends.Game.Hexmaps;

/// <summary>
/// Position in hex coordinates.
/// Uses offset coordinates
/// </summary>
struct OffsetCoord
{
    /// <summary>
    /// Describes which rows of the hex grid are shoved to the right.
    /// </summary>
    public enum ShoveRowsRight
    {
        Even = 1,
        Ordd = -1
    }
    
    public readonly int Col;
    public readonly int Row;

    public OffsetCoord(int col, int row)
    {
        Col = col;
        Row = row;
    }

    /// <summary>
    /// Transforms cube coordinates to offset (even/odd) Q-coordinates
    /// </summary>
    /// <remarks>
    /// Q-coordinates describe flat-top hexes
    /// </remarks>
    static public OffsetCoord QoffsetFromCube(ShoveRowsRight offset, Hex h)
    {
        int col = h.Q;
        int row = h.R + (h.Q + (int)offset * (h.Q & 1)) / 2;
        return new OffsetCoord(col, row);
    }

    /// <summary>
    /// Transforms offset even/odd Q-coordinates to cube coordinates
    /// </summary>
    /// <remarks>
    /// Q-coordinates describe flat-top hexes
    /// </remarks>
    static public Hex QoffsetToCube(ShoveRowsRight offset, OffsetCoord h)
    {
        int q = h.Col;
        int r = h.Row - (h.Col + (int)offset * (h.Col & 1)) / 2;
        int s = -q - r;
        return new Hex(q, r, s);
    }

    /// <summary>
    /// Transforms cube coordinates to offset (even/odd) R-coordinates
    /// </summary>
    /// <remarks>
    /// R-coordinates describe spiky-top hexes
    /// </remarks>
    static public OffsetCoord RoffsetFromCube(ShoveRowsRight offset, Hex h)
    {
        int col = h.Q + (h.R + (int)offset * (h.R & 1)) / 2;
        int row = h.R;
        return new OffsetCoord(col, row);
    }

    /// <summary>
    /// Transforms offset (even/odd) R-coordinates to cube coordinates
    /// </summary>
    /// <remarks>
    /// R-coordinates describe spiky-top hexes
    /// </remarks>
    static public Hex RoffsetToCube(ShoveRowsRight offset, OffsetCoord h)
    {
        int q = h.Col - (h.Row + (int)offset * (h.Row & 1)) / 2;
        int r = h.Row;
        int s = -q - r;
        return new Hex(q, r, s);
    }
}