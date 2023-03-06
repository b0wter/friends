using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Friends.Game.Helpers.Tiled;

public class PropertyTypeConverter : JsonConverter<PropertyType>
{
    public override void WriteJson(JsonWriter writer, PropertyType value, JsonSerializer serializer)
    {
        throw new NotImplementedException("This converter is not meant to write json");
    }

    public override PropertyType ReadJson(JsonReader reader, Type objectType, PropertyType existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var json = JObject.Load(reader);
        var property =
            json.GetValue("type")!.Value<string>().ToUpperInvariant() switch
            {
                "ENUM" => (PropertyType)new EnumPropertyType(),
                "CLASS" => new ClassType(),
                { } s => throw new ArgumentException($"Cannot deserialize PropertyType of type '{s}'")
            };
        /*
        var type =
            json.GetValue("type")!.Value<string>().ToUpperInvariant() switch
            {
                "enum" => typeof(EnumPropertyType),
                { } s => throw new ArgumentException($"Cannot deserialize PropertyType of type '{s}'")
            };
        var contract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(type);
        var propertyType = existingValue as PropertyType ?? (PropertyType)contract.DefaultCreator!();
        */
        using (var subReader = json.CreateReader())
        {
            serializer.Populate(subReader, property);
        }

        return property;
    }
}