namespace Friends.Lib

open System

module Group =
    type Group = {
        Id: Guid
        Name: string
        Description: string option
    }

    let id g = g.Id

    let name g = g.Name

    let description g = g.Description

    /// <summary>
    /// Contains helper methods for the creation of `Group` records.
    /// </summary>
    /// <remarks>
    /// Uses a static factory pattern because member methods allow optional parameters<br/>
    /// Could also be added as static function to `Group` itself but typing `Group.Group.Create` feels strange
    /// </remarks>
    [<AbstractClass; Sealed>]
    type Factory private () =
        static member Create (name, ?description, ?id) =
            {
                Id = id |> Option.defaultValue (Guid.NewGuid ())
                Name = name
                Description = description
            }

