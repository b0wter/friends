namespace Friends.Lib

open Friends.Graph

module SocialGraphs =
    
    type SocialGraph() =
        inherit Graphs.Graph<Connections.GraphNode, Connections.Connection>(3)
        
        let mergePaths (paths: (Connections.GraphNode * Connections.Connection) list list) : RelationshipRenown.T * RelationshipScore.T =
            
            let mergePath (path: (Connections.GraphNode * Connections.Connection) list) =
                (*
                    The social graph uses a maximal search path length of 3. This results in combinations that are consciously ignored
                    (e.g. chains of three or more persons)
                
                    The following relationships are considered:
                    
                    From --> Person --> To
                    From --> To
                    From -|Member|-> Group --> Group <-|Member|- To
                    From -|Member|-> Group <-|Member|- To
                    From --> Group <-|Member|- To
                *)
                match path with
                // From --> Person --> To
                | [ Connections.GraphNode.Person _, Connections.Connection.Values (renown1, score1); Connections.GraphNode.Person _, Connections.Connection.Values (renown2, score2) ] ->
                    (
                        let factor = (renown1 |> RelationshipRenown.asFactor) * (renown2 |> RelationshipRenown.asFactor)
                        RelationshipRenown.decimalMinValue + factor * RelationshipRenown.decimalMaxValue |> RelationshipRenown.createClamped,
                        RelationshipScore.factor score2 (RelationshipScore.asFactor score1)
                    ) |> Some
                // From --> To
                | [ Connections.GraphNode.Person _, Connections.Connection.Values c1 ] ->
                    Some c1
                // From -|Member|-> Group --> Group <-|Member|- To
                | [ Connections.Group _, Connections.Connection.MemberOf; Connections.Group _, Connections.Connection.Values c1; Connections.Person _, Connections.Connection.MemberOf ] ->
                    Some c1
                // From -|Member|-> Group <-|Member|- To
                | [ Connections.Group _, Connections.Connection.MemberOf; Connections.Person _, Connections.Connection.HasMember ] ->
                    (RelationshipRenown.minValue, RelationshipScore.defaultGroupValue) |> Some
                // From --> Group <-|Member|- To
                | [ Connections.Group _, Connections.Connection.Values (_, score); Connections.Person _, Connections.Connection.HasMember ] ->
                    (RelationshipRenown.minValue, 0.5m * score) |> Some
                | _ ->
                    None

            let renowns, scores =
                match paths |> List.map mergePath |> List.choose id with
                | [] -> [ (RelationshipRenown.minValue, RelationshipScore.defaultValue) ]
                | many -> many
                |> List.unzip
            
            (renowns |> List.max, scores |> List.map RelationshipScore.value |> List.average |> RelationshipScore.createClamped)
            
        
        member this.Add person =
            (this :> Graphs.IGraph<Connections.GraphNode, Connections.Connection>).Add (Connections.Person person)
            
        member this.Add group =
            (this :> Graphs.IGraph<Connections.GraphNode, Connections.Connection>).Add (Connections.Group group)
            
        member this.Remove person =
            (this :> Graphs.IGraph<Connections.GraphNode, Connections.Connection>).Remove (Connections.Person person)
            
        member this.Remove group =
            (this :> Graphs.IGraph<Connections.GraphNode, Connections.Connection>).Remove (Connections.Group group)
            
        member this.Update (person1, person2, value) =
            (this :> Graphs.IGraph<Connections.GraphNode, Connections.Connection>).Update
                (Connections.Person person1)
                (Connections.Person person2)
                value
            
        member this.Update (person, group, value) =
            do (this :> Graphs.IGraph<Connections.GraphNode, Connections.Connection>).Update
                (Connections.Person person)
                (Connections.Group group)
                value
            match value with
            | Connections.Connection.MemberOf ->
                // When adding a membership two connections have to be added to be able to find the connection:
                // Person -|Member|-> Group --> Group <-|Member|- Person
                (this :> Graphs.IGraph<Connections.GraphNode, Connections.Connection>).Update
                    (Connections.Group group)
                    (Connections.Person person)
                    Connections.Connection.HasMember
            | _ -> ()
            
        member this.Update (group, person, value) =
            do (this :> Graphs.IGraph<Connections.GraphNode, Connections.Connection>).Update
                (Connections.Group group)
                (Connections.Person person)
                value
            match value with
            | Connections.Connection.HasMember ->
                // When adding a membership two connections have to be added to be able to find the connection:
                // Person -|Member|-> Group --> Group <-|Member|- Person
                (this :> Graphs.IGraph<Connections.GraphNode, Connections.Connection>).Update                
                    (Connections.Person person)                
                    (Connections.Group group)                
                    Connections.Connection.MemberOf
            | _ -> ()
                    
        member this.Update (group1, group2, value) =
            (this :> Graphs.IGraph<Connections.GraphNode, Connections.Connection>).Update
                (Connections.Group group1)
                (Connections.Group group2)
                value
            
        member this.FindAllPaths (person1, person2) =
            (this :> Graphs.IGraph<Connections.GraphNode, Connections.Connection>).FindAllPaths
                (Connections.Person person1)
                (Connections.Person person2)
            
        member this.FindRenownAndScoreFor(person1: Person.Person, person2: Person.Person) =
            this.FindAllPaths (person1, person2)
            |> mergePaths
            
        member this.FindAllPaths (person, group) =
            (this :> Graphs.IGraph<Connections.GraphNode, Connections.Connection>).FindAllPaths
                (Connections.Person person)
                (Connections.Group group)
            
        member this.FindRenownAndScoreFor(person: Person.Person, group: Group.Group) =
            this.FindAllPaths (person, group)
            |> mergePaths
            
        member this.FindAllPaths (group, person) =
            (this :> Graphs.IGraph<Connections.GraphNode, Connections.Connection>).FindAllPaths
                (Connections.Group group)
                (Connections.Person person)
            
        member this.FindRenownAndScoreFor(group: Group.Group, person: Person.Person) =
            this.FindAllPaths (group, person)
            |> mergePaths
            
        member this.FindAllPaths (group1, group2) =
            (this :> Graphs.IGraph<Connections.GraphNode, Connections.Connection>).FindAllPaths
                (Connections.Group group1)
                (Connections.Group group2)
                
        member this.FindRenownAndScoreFor(group1: Group.Group, group2: Group.Group) =
            this.FindAllPaths (group1, group2)
            |> mergePaths
            
