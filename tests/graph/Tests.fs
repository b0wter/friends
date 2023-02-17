module Tests

open System
open Xunit
open Friends.Graph.Graphs
open FsUnit.Xunit

[<Fact>]
let ``Searching for all paths through a graph returns all possible paths (non-complex graph)`` () =
    (*
    Graph looks like this:
        1
       / \
      2   3
      | X |     // X is a cross from 2 -> 5 and 3 -> 4
      4   5
       \ /
        6
    let mermaid = graph.AsMermaidGraph(fun x -> "Id_" + (x |> string))
    *)
    let graph = Graph<int, string>() :> IGraph<int, string>
    graph.Add 1
    graph.Add 2
    graph.Add 3
    graph.Add 4
    graph.Add 5
    graph.Add 6
    graph.Update 1 2 "1-2"
    graph.Update 1 3 "1-3"
    graph.Update 3 5 "3-5"
    graph.Update 5 6 "5-6"
    graph.Update 2 4 "2-4"
    graph.Update 4 6 "4-6"
    graph.Update 2 5 "2-5"
    graph.Update 3 4 "3-4"
    
    let paths = graph.FindAllPathsWith 1 6 99
    
    paths |> should haveLength 4
    paths |> List.iter (fun p -> p |> should haveLength 3)
    paths |> should be unique


[<Fact>]
let ``Searching a path between two unconnected nodes returns empty list`` () =
    let graph = Graph<int, string>() :> IGraph<int, string>
    graph.Add 1
    graph.Add 2
    
    let paths = graph.FindAllPaths 1 2
    
    paths |> should be Empty


[<Fact>]
let ``Searching for a path in a graph with a self-referencing does not result in a deadlock`` () =
    let graph = Graph<int, string>() :> IGraph<int, string>
    graph.Add 1
    graph.Add 2
    graph.Add 3
    graph.Add 4
    graph.Update 1 2 "1-2"
    graph.Update 2 3 "2-3"
    graph.Update 3 3 "whee!"
    graph.Update 3 4 "3-4"
    
    let paths = graph.FindAllPathsWith 1 4 99
    
    paths |> should haveLength 1
    paths |> List.head |> should haveLength 3
