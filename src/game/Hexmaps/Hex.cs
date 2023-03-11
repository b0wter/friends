using System;
using System.Collections.Generic;

namespace Friends.Game.Hexmaps;

/// <summary>
/// Position in hex coordinates.
/// Uses cubic coordinates
/// </summary>
struct Hex
{
    public Hex(int q, int r, int s)
    {
        Q = q;
        R = r;
        S = s;
        if (q + r + s != 0) throw new ArgumentException("q + r + s must be 0");
    }
    public readonly int Q;
    public readonly int R;
    public readonly int S;

    public Hex Add(Hex b)
    {
        return new Hex(Q + b.Q, R + b.R, S + b.S);
    }


    public Hex Subtract(Hex b)
    {
        return new Hex(Q - b.Q, R - b.R, S - b.S);
    }


    public Hex Scale(int k)
    {
        return new Hex(Q * k, R * k, S * k);
    }


    public Hex RotateLeft()
    {
        return new Hex(-S, -Q, -R);
    }


    public Hex RotateRight()
    {
        return new Hex(-R, -S, -Q);
    }

    static public List<Hex> directions = new() {new Hex(1, 0, -1), new Hex(1, -1, 0), new Hex(0, -1, 1), new Hex(-1, 0, 1), new Hex(-1, 1, 0), new Hex(0, 1, -1)};

    static public Hex Direction(int direction)
    {
        return directions[direction];
    }


    public Hex Neighbor(int direction)
    {
        return Add(Direction(direction));
    }

    static public List<Hex> diagonals = new() {new Hex(2, -1, -1), new Hex(1, -2, 1), new Hex(-1, -1, 2), new Hex(-2, 1, 1), new Hex(-1, 2, -1), new Hex(1, 1, -2)};

    public Hex DiagonalNeighbor(int direction)
    {
        return Add(diagonals[direction]);
    }


    public int Length()
    {
        return (Math.Abs(Q) + Math.Abs(R) + Math.Abs(S)) / 2;
    }


    public int Distance(Hex b)
    {
        return Subtract(b).Length();
    }

}
