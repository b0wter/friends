namespace Friends.Lib

open System

module Person =
   
    type Person = {
        Id: Guid
        FirstName: string
        LastName: string option
        MiddleName : string option
        Age: uint
        Groups: Group list
        GroupRelationships: Relationship.Relationship<Person, Group> list
        PersonRelationships: Relationship.Relationship<Person, Person> list
    }
    and Group = {
        Id: Guid
        Name: string
        Description: string option
        GroupRelationships: Relationship.Relationship<Person, Group> list
        PersonRelationships: Relationship.Relationship<Person, Person> list
    }
