using System;
using System.Collections.Generic;

namespace Friends.Game.Hexmaps;

/// <summary>
/// Defines the layout of the hex grid (e.g. orientation).
/// Can be used to translate hex coordinates to pixels and vice versa
/// </summary>
struct Layout
{
    /// <summary>
    /// Orientation of the given layout. May either be ⬢ or ⬣
    /// </summary>
    public readonly Orientation Orientation;
    
    /// <summary>
    /// Size of a single hex field
    /// </summary>
    public readonly Point Size;
    
    /// <summary>
    /// Position of the 0,0 hex. Works like an offset
    /// </summary>
    public readonly Point Origin;
    
    /// <summary>
    /// Hexagons are standing on a corner: ⬢
    /// </summary>
    static public Orientation Pointy = new(Math.Sqrt(3.0), Math.Sqrt(3.0) / 2.0, 0.0, 3.0 / 2.0, Math.Sqrt(3.0) / 3.0, -1.0 / 3.0, 0.0, 2.0 / 3.0, 0.5);

    /// <summary>
    /// Hexagons are standing on an edge: ⬣
    /// </summary>
    static public Orientation Flat = new(3.0 / 2.0, 0.0, Math.Sqrt(3.0) / 2.0, Math.Sqrt(3.0), 2.0 / 3.0, 0.0, -1.0 / 3.0, Math.Sqrt(3.0) / 3.0, 0.0);
    
    public Layout(Orientation orientation, Point size, Point origin)
    {
        this.Orientation = orientation;
        this.Size = size;
        this.Origin = origin;
    }

    public Point HexToPixel(Hex h)
    {
        Orientation M = Orientation;
        double x = (M.f0 * h.Q + M.f1 * h.R) * Size.x;
        double y = (M.f2 * h.Q + M.f3 * h.R) * Size.y;
        return new Point(x + Origin.x, y + Origin.y);
    }


    public FractionalHex PixelToHex(Point p)
    {
        Orientation M = Orientation;
        Point pt = new Point((p.x - Origin.x) / Size.x, (p.y - Origin.y) / Size.y);
        double q = M.b0 * pt.x + M.b1 * pt.y;
        double r = M.b2 * pt.x + M.b3 * pt.y;
        return new FractionalHex(q, r, -q - r);
    }


    public Point HexCornerOffset(int corner)
    {
        Orientation M = Orientation;
        double angle = 2.0 * Math.PI * (M.start_angle - corner) / 6.0;
        return new Point(Size.x * Math.Cos(angle), Size.y * Math.Sin(angle));
    }


    public List<Point> PolygonCorners(Hex h)
    {
        var corners = new List<Point>(6);
        Point center = HexToPixel(h);
        for (int i = 0; i < 6; i++)
        {
            Point offset = HexCornerOffset(i);
            corners.Add(new Point(center.x + offset.x, center.y + offset.y));
        }
        return corners;
    }

}
