namespace Friends.Lib

module Paths =
    
    type Path = (Connections.GraphNode * Connections.Connection) list
    
    let merge (paths: (Connections.GraphNode * Connections.Connection) list list) : RelationshipRenown.T * RelationshipScore.T =
        
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
                    RelationshipScore.scale score2 (RelationshipScore.asFactor score1)
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
