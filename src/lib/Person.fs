namespace Friends.Lib

open System

module Person =
    
    type Person = {
        Id: Guid
        FirstName: string
        LastName: string option
        MiddleName : string option
        Age: uint
        Groups: {| Group: Group; Significance: SignificanceInGroup.T |} list
        GroupRelationships: Relationship.Relationship<Group> list
        PersonRelationships: Relationship.Relationship<Person> list
        PlayerRenown : RelationshipRenown.T
    }
    and Group = {
        Id: Guid
        Name: string
        Description: string option
        GroupRelationships: Relationship.Relationship<Group> list
        PersonRelationships: Relationship.Relationship<Person> list
        PlayerRenown : RelationshipRenown.T
    }

    /// <summary>
    /// Contains helper methods for the creation of `Person` records.
    /// </summary>
    /// <remarks>
    /// Uses a static factory pattern because member methods allow optional parameters
    /// </remarks>
    [<AbstractClass; Sealed>]
    type PersonFactory private () =
        static member Create (age, firstname, ?middlename, ?lastname, ?groups, ?groupRelations, ?personRelations, ?playerRenown, ?id) =
            {
                Age = age
                Groups = groups |> Option.defaultValue []
                Id = id |> Option.defaultValue (Guid.NewGuid ())
                LastName = lastname
                FirstName = firstname
                MiddleName = middlename
                GroupRelationships = groupRelations |> Option.defaultValue []
                PersonRelationships = personRelations |> Option.defaultValue []
                PlayerRenown = playerRenown |> Option.defaultValue RelationshipRenown.defaultValue
            }

    /// <summary>
    /// Contains helper methods for the creation of `Group` records.
    /// </summary>
    /// <remarks>
    /// Uses a static factory pattern because member methods allow optional parameters
    /// </remarks>
    [<AbstractClass; Sealed>]
    type GroupFactory private () =
        static member Create (name, ?description, ?groupRelations, ?personRelations, ?playerRenown, ?id) =
            {
                Id = id |> Option.defaultValue (Guid.NewGuid ())
                Name = name
                Description = description
                GroupRelationships = groupRelations |> Option.defaultValue []
                PersonRelationships = personRelations |> Option.defaultValue []
                PlayerRenown = playerRenown |> Option.defaultValue RelationshipRenown.defaultValue
            }
