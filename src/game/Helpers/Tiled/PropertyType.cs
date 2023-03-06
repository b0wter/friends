using Newtonsoft.Json;

namespace Friends.Game.Helpers.Tiled;

[JsonConverter(typeof(PropertyTypeConverter))]
public abstract class PropertyType
{
    public int Id { get; set; }
    
    public string Name { get; set; }
}