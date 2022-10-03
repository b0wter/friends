namespace Friends.Lib.Tests

open Friends.Lib
open Xunit
open FsUnit.Xunit
open System.Linq

module SparseMatrixTests =
    
    [<Fact>]
    let ``Getting an existing element returns it`` () =
        let matrix = SparseMatrix.create<int, int> 1
        42 |> SparseMatrix.set 2 2 matrix
        
        matrix |> SparseMatrix.get 2 2 |> should equal 42
    
    [<Fact>]
    let ``Getting a non-existing element returns fallback value`` () =
        let matrix = SparseMatrix.create<int, int> 1
        
        matrix |> SparseMatrix.get 2 2 |> should equal 1

    [<Fact>]
    let ``Removing and existing element will return fallback value in subsequent call`` () =
        let matrix = SparseMatrix.create<int, int> 1
        42 |> SparseMatrix.set 2 2 matrix
        
        matrix |> SparseMatrix.get 2 2 |> should equal 42
        matrix |> SparseMatrix.remove 2 2 |> should equal true
        matrix |> SparseMatrix.get 2 2 |> should equal 1

    [<Fact>]
    let ``Applying a function to an existing element will change its value`` () =
        let matrix = SparseMatrix.create<int, int> 1
        42 |> SparseMatrix.set 2 2 matrix
        
        matrix |> SparseMatrix.apply 2 2 ((*) 2) |> should be True
        
        matrix |> SparseMatrix.get 2 2 |> should equal 84
        
    [<Fact>]
    let ``Applying a function to a non-existing element will return false and change nothing`` () =
        let matrix = SparseMatrix.create<int, int> 1
        42 |> SparseMatrix.set 2 2 matrix
        
        matrix |> SparseMatrix.apply 3 3 ((*) 2) |> should be False
        
        matrix |> SparseMatrix.get 2 2 |> should equal 42
        matrix |> SparseMatrix.get 3 3 |> should equal 1
        
    [<Fact>]
    let ``Retrieving a row with set values returns all non-default values`` () =
        let matrix = SparseMatrix.create<int, int> 1
        2 |> SparseMatrix.set 2 1 matrix
        3 |> SparseMatrix.set 2 2 matrix
        
        let row = matrix |> SparseMatrix.row 2 |> List.ofSeq
        
        row |> should haveLength 2
        row |> should be (supersetOf [ 2; 3 ])
        
    [<Fact>]
    let ``Retrieving a row with no values returns empty result`` () =
        let matrix = SparseMatrix.create<int, int> 1
        2 |> SparseMatrix.set 2 1 matrix
        3 |> SparseMatrix.set 2 2 matrix
        
        let row = matrix |> SparseMatrix.row 1 |> List.ofSeq
        
        row |> should be Empty
        
    [<Fact>]
    let ``Retrieving a col with set values returns all non-default values`` () =
        let matrix = SparseMatrix.create<int, int> 1
        2 |> SparseMatrix.set 1 2 matrix
        3 |> SparseMatrix.set 2 2 matrix
        
        let row = matrix |> SparseMatrix.col 2 |> List.ofSeq
        
        row |> should haveLength 2
        row |> should be (subsetOf [ 2; 3 ])
        
    [<Fact>]
    let ``Retrieving a col with no values returns empty result`` () =
        let matrix = SparseMatrix.create<int, int> 1
        2 |> SparseMatrix.set 1 2 matrix
        3 |> SparseMatrix.set 2 2 matrix
        
        let row = matrix |> SparseMatrix.col 1 |> List.ofSeq
        
        row |> should be Empty
