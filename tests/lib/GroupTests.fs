namespace Friends.Lib.Tests

open System
open Xunit
open FsUnit.Xunit
open Friends.Lib
open Friends.Lib.Person

module GroupTests =
    
    [<Fact>]
    let ``foobar`` () =
        let cityGuards =
            GroupFactory.Create (name = "City Guard")

        let royalGuards =
            GroupFactory.Create (name = "Royal Guard")
            
        let cityGuards =
            {
                cityGuards with
                    GroupRelationships = [
                        {
                            Relationship.Relationship.Score = RelationshipScore.defaultValue
                            Relationship.Relationship.Towards = royalGuards
                        }
                    ]
            }
            
        let cityGuard1 =
            PersonFactory.Create (
                age = 20u,
                firstname = "Bob",
                groups = [ {| Group = cityGuards; Significance = SignificanceInGroup.defaultValue |} ]
            )
            
        let cityGuard2 =
            PersonFactory.Create (
                age = 21u,
                firstname = "Peter",
                groups = [ {| Group = cityGuards; Significance = SignificanceInGroup.defaultValue |} ]
            )
            
        let royalGuards =
            {
                royalGuards with
                    GroupRelationships = [
                        {
                            Relationship.Relationship.Score = RelationshipScore.defaultValue + (RelationshipScore.createClamped 0.2m)
                            Relationship.Relationship.Towards = cityGuards
                        }
                    ]
            }
            
        ()
