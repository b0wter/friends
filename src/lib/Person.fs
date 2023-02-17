namespace Friends.Lib

open System

module Person =
    type Person = {
        Id: Guid
        FirstName: string
        LastName: string option
        MiddleName : string option
        Age: uint
    }
    
    let id p = p.Id
    
    let firstName p = p.FirstName
    
    let lastName p = p.LastName
    
    let middleName p = p.MiddleName
    
    let age p = p.Age
    
    /// <summary>
    /// Contains helper methods for the creation of `Person` records.
    /// </summary>
    /// <remarks>
    /// Uses a static factory pattern because member methods allow optional parameters
    /// </remarks>
    [<AbstractClass; Sealed>]
    type Factory private () =
        static member Create (age, firstName, ?middleName, ?lastName, ?id) =
            {
                Age = age
                Id = id |> Option.defaultValue (Guid.NewGuid ())
                LastName = lastName
                FirstName = firstName
                MiddleName = middleName
            }
