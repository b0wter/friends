namespace Friends.Lib

open System.Collections.Generic
open System.Linq

module SparseMatrix =
    type T<'key, 'value> = {
        // Composite key: `from` * `to`
        Values: Dictionary<'key * 'key, 'value>
        SparseValue: 'value
    }
    
    let create<'key, 'value when 'key : equality> defaultValue =
        {
            Values = Dictionary<'key * 'key, 'value>(5000)
            SparseValue = defaultValue
        }
        
    let get row col t =
        if t.Values.ContainsKey(row, col) then t.Values[row, col]
        else t.SparseValue
        
    let row rowNumber t =
        let keys = t.Values.Keys |> Seq.where (fun pair -> pair |> fst = rowNumber)
        keys |> Seq.map (fun key -> t.Values[key])
        
    let col colNumber t =
        let keys = t.Values.Keys |> Seq.where (fun pair -> pair |> snd = colNumber)
        keys |> Seq.map (fun key -> t.Values[key])
        
    let set<'key, 'value when 'key : equality> row col (t: T<'key, 'value>) value =
        t.Values[(row, col)] <- value
        
    let remove row col t =
        t.Values.Remove((row, col))
        
    let apply row col f t =
        if t.Values.ContainsKey((row, col)) then
            do t.Values[(row, col)] <- f t.Values[(row, col)]
            true
        else false
 
    (*
    type T<'key, 'value> = {
        Rows: Dictionary<'key, Dictionary<'key, 'value>>
        SparseValue: 'value
    }
    
    let create<'key, 'value when 'key : equality> defaultValue =
        {
            Rows = Dictionary<'key, Dictionary<'key, 'value>>()
            SparseValue = defaultValue
        }
    
    let get row col t =
        if t.Rows.ContainsKey(row) && t.Rows[row].ContainsKey(col) then t.Rows[row][col]
        else t.SparseValue

    let row<'key, 'value when 'key : equality> rowNumber (t: T<'key, 'value>) =
        if t.Rows.ContainsKey(rowNumber) then t.Rows[rowNumber]
        else Dictionary<'key, 'value>()
    
    let set<'key, 'value when 'key : equality> row col (t: T<'key, 'value>) value =
        if not <| (t.Rows.ContainsKey row) then
            t.Rows.Add(row, Dictionary<'key, 'value>())
        else ()
        t.Rows[row][col] <- value
    
    let remove row col t =
        if t.Rows.ContainsKey row then
            t.Rows[row].Remove col
        else
            false
            
    let apply row col f t =
        if t.Rows.ContainsKey row && t.Rows[row].ContainsKey col then
            do t.Rows[row][col] <- f (t.Rows[row][col])
            true
        else false
    *)
        
    (*
    type T<'key, 'value> = {
        Cols: Dictionary<'key, Dictionary<'key, 'value>>
        SparseValue: 'value
    }
    
    let create<'key, 'value when 'key : equality> defaultValue =
        {
            Cols = Dictionary<'key, Dictionary<'key, 'value>>()
            SparseValue = defaultValue
        }
    
    let get row col t =
        if t.Cols.ContainsKey(col) && t.Cols[col].ContainsKey(row) then t.Cols[col][row]
        else t.SparseValue

    let col<'key, 'value when 'key : equality> rowNumber (t: T<'key, 'value>) =
        if t.Cols.ContainsKey(rowNumber) then t.Cols[rowNumber]
        else Dictionary<'key, 'value>()
    
    let row<'key, 'value when 'key : equality> rowNumber (t: T<'key, 'value>) =
        let pairs =
            t.Cols
            |> Seq.map (fun col -> col.Value |> Seq.tryFind (fun pair -> pair.Key = rowNumber))
            |> Seq.choose id
        Dictionary(pairs)
        
    let set<'key, 'value when 'key : equality> row col (t: T<'key, 'value>) value =
        if not <| (t.Cols.ContainsKey col) then
            t.Cols.Add(col, Dictionary<'key, 'value>())
        else ()
        t.Cols[col][row] <- value
    
    let remove row col t =
        if t.Cols.ContainsKey col then
            t.Cols[col].Remove row
        else
            false
            
    let apply row col f t =
        if t.Cols.ContainsKey col && t.Cols[col].ContainsKey row then
            do t.Cols[col][row] <- f (t.Cols[col][row])
            true
        else false
        *)
