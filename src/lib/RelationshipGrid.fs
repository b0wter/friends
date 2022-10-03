namespace Friends.Lib

open System

module RelationshipGrid =
    
    type RelationshipGrid = {
        Scores: SparseMatrix.T<Guid, RelationshipScore.T>
    }

    let create defaultValue =
        {
            Scores = SparseMatrix.create defaultValue
        }
        
    let get from ``to`` grid =
        grid.Scores |> SparseMatrix.get from ``to``