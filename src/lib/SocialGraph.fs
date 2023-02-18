namespace Friends.Lib

open Friends.Graph

module SocialGraphs =
    
    type SocialGraph() =
        inherit Graphs.Graph<Connections.GraphNode, Connections.Connection>(3)
        
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
            
        member this.FindAllPaths (person1, person2) : Paths.Path list =
            (this :> Graphs.IGraph<Connections.GraphNode, Connections.Connection>).FindAllPaths
                (Connections.Person person1)
                (Connections.Person person2)
            
        member this.FindRenownAndScoreFor(person1: Person.Person, person2: Person.Person) =
            this.FindAllPaths (person1, person2)
            |> Paths.merge
            
        member this.FindAllPaths (person, group) : Paths.Path list =
            (this :> Graphs.IGraph<Connections.GraphNode, Connections.Connection>).FindAllPaths
                (Connections.Person person)
                (Connections.Group group)
            
        member this.FindRenownAndScoreFor(person: Person.Person, group: Group.Group) =
            this.FindAllPaths (person, group)
            |> Paths.merge
            
        member this.FindAllPaths (group, person) : Paths.Path list =
            (this :> Graphs.IGraph<Connections.GraphNode, Connections.Connection>).FindAllPaths
                (Connections.Group group)
                (Connections.Person person)
            
        member this.FindRenownAndScoreFor(group: Group.Group, person: Person.Person) =
            this.FindAllPaths (group, person)
            |> Paths.merge
            
        member this.FindAllPaths (group1, group2) : Paths.Path list =
            (this :> Graphs.IGraph<Connections.GraphNode, Connections.Connection>).FindAllPaths
                (Connections.Group group1)
                (Connections.Group group2)
                
        member this.FindRenownAndScoreFor(group1: Group.Group, group2: Group.Group) =
            this.FindAllPaths (group1, group2)
            |> Paths.merge
            
