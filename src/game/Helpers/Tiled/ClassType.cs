using System.Collections.Generic;
using System.Dynamic;

namespace Friends.Game.Helpers.Tiled;

public class ClassType : PropertyType
{
    public class ClassMember
    {
        public string Name { get; init; }
        
        public string PropertyType { get; init; }
        
        public string Type { get; init; }

        public string Value { get; init; }
    }
    
    public List<ClassMember> Members { get; set; }
}