namespace Friends.Lib.Tests

open Friends.Lib
open Friends.Lib.SocialGraphs
open FsUnit.Xunit
open Xunit

module SocialGraphTests =
    
    [<Fact>]
    let ``Finding a path from a person in a group to a person in another group returns a single path`` () =
        let graph = SocialGraph()
        
        let cityGuards =
            Group.Factory.Create (name = "City Guard")
        graph.Add cityGuards

        let royalGuards =
            Group.Factory.Create (name = "Royal Guard")
        graph.Add royalGuards
            
        let cityGuard1 =
            Person.Factory.Create (
                age = 20u,
                firstName = "Bob"
            )
        graph.Update (cityGuard1, cityGuards, Connections.MemberOf)
            
        let cityGuard2 =
            Person.Factory.Create (
                age = 21u,
                firstName = "Peter"
            )
        graph.Update (cityGuard2, cityGuards, Connections.MemberOf)
        
        let royalGuard1 =
            Person.Factory.Create (
                age = 31u,
                firstName = "Justus"
            )
        graph.Update (royalGuard1, royalGuards, Connections.MemberOf)
        
        graph.Update (cityGuards, royalGuards, Connections.Values (RelationshipRenown.maxValue, RelationshipScore.maxValue))
        graph.Update (royalGuards, cityGuards, Connections.Values (RelationshipRenown.maxValue, RelationshipScore.minValue))
            
        let paths = graph.FindAllPaths (cityGuard1, royalGuard1)
        paths |> should haveLength 1

    
    [<Fact>]
    let ``Finding a path from a person in a group to a person in another group that also have a direct connection returns two paths`` () =
        let graph = SocialGraph()
        
        let cityGuards =
            Group.Factory.Create (name = "City Guard")
        graph.Add cityGuards

        let royalGuards =
            Group.Factory.Create (name = "Royal Guard")
        graph.Add royalGuards
            
        let cityGuard1 =
            Person.Factory.Create (
                age = 20u,
                firstName = "Bob"
            )
        graph.Update (cityGuard1, cityGuards, Connections.MemberOf)
            
        let cityGuard2 =
            Person.Factory.Create (
                age = 21u,
                firstName = "Peter"
            )
        graph.Update (cityGuard2, cityGuards, Connections.MemberOf)
        
        let royalGuard1 =
            Person.Factory.Create (
                age = 31u,
                firstName = "Justus"
            )
        graph.Update (royalGuard1, royalGuards, Connections.MemberOf)
        
        graph.Update (cityGuards, royalGuards, Connections.Values (RelationshipRenown.maxValue, RelationshipScore.maxValue))
        graph.Update (royalGuards, cityGuards, Connections.Values (RelationshipRenown.maxValue, RelationshipScore.minValue))
        graph.Update (cityGuard1, royalGuard1, Connections.Values (RelationshipRenown.defaultValue, RelationshipScore.defaultValue))
            
        let paths = graph.FindAllPaths (cityGuard1, royalGuard1)
        paths |> should haveLength 2
