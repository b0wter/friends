namespace Friends.Graph

open System.Linq
open System.Text

module Graphs =
    open System.Collections.Generic

    type IGraph<'node, 'value> =
        abstract Add : 'node -> unit
        abstract Remove : 'node -> unit
        abstract Update : 'node -> 'node -> 'value -> unit
        abstract FindAllPaths : 'node -> 'node -> ('node * 'value) list list
        abstract FindAllPathsWith : 'node -> 'node -> int -> ('node * 'value) list list
    
    /// <summary>
    /// Describes a complete graph consisting of many nodes and connections (/edges).
    /// Offers functionality to search paths from one node to another and to add/remove/update connections (/edges)
    /// </summary>
    type Graph<'node, 'value when 'node: equality>(defaultSearchDepth: int) =
        let adjacsencies = Dictionary<'node, Dictionary<'node, 'value>>()
        
        new() = Graph(99)

            
        member this.AsMermaidGraph(idGenerator: 'node -> string) =
            let builder = StringBuilder()
            do builder.AppendLine "graph TD" |> ignore

            for pair in adjacsencies do
                if pair.Value.Any() then
                    for connection in pair.Value do
                        builder.AppendLine
                            $"%O{pair.Key |> idGenerator} --> |%A{connection.Value}| %O{connection.Key |> idGenerator}"
                        |> ignore
                else
                    builder.AppendLine $"%O{pair.Key |> idGenerator}" |> ignore

            builder.ToString()
                
                
        interface IGraph<'node, 'value> with
            member this.Add node =
                if node |> adjacsencies.ContainsKey then
                    ()
                else
                    adjacsencies.Add(node, Dictionary<'node, 'value>())

            
            member this.Remove node =
                do adjacsencies.Remove node |> ignore

                for pair in adjacsencies do
                    pair.Value.Remove node |> ignore

            
            member this.Update from ``to`` (newValue: 'value) =
                let fromExists, connections = adjacsencies.TryGetValue from

                if fromExists then
                    connections[``to``] <- newValue
                else
                    let connections = Dictionary<'node, 'value>()
                    do connections.Add(``to``, newValue)
                    do adjacsencies.Add(from, connections)


            member this.FindAllPathsWith (from: 'node) (``to``: 'node) maxDepth : ('node * 'value) list list =
                let rec step (current: 'node) (currentPath: ('node * 'value) list) currentDepth (accumulator: ('node * 'value) list list) =
                    if currentDepth > maxDepth then
                        accumulator
                    else if current = ``to`` then
                        (currentPath |> List.rev) :: accumulator
                    else
                        let connections =
                            if current |> adjacsencies.ContainsKey then
                                adjacsencies[current]
                                // Filter all nodes that have previously been visited
                                |> Seq.filter (fun connection ->
                                    (connection.Key <> from) && (currentPath |> (not << List.exists (fun (p, _) -> p = connection.Key))))
                            else Seq.empty
                        connections
                        |> Seq.map (fun c -> step c.Key ((c.Key, c.Value) :: currentPath) (currentDepth + 1) accumulator)
                        |> Seq.collect id
                        |> List.ofSeq
                step from [] 0 []

            member this.FindAllPaths (from: 'node) (``to``: 'node) : ('node * 'value) list list =
                (this :> IGraph<'node, 'value>).FindAllPathsWith from ``to`` defaultSearchDepth
