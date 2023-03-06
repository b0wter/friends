using System.Collections.Generic;

namespace Friends.Game.Helpers.Tiled;

public class EnumPropertyType : PropertyType
{
    public List<string> Values { get; set; } = new();
}