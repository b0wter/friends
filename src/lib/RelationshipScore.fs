namespace Friends.Lib

open System

module RelationshipScore =
    let decimalMinValue = 0m
    let decimalMaxValue = 2m
    let decimalDefaultValue = 1m
    let private decimalMiddleValue = (decimalMaxValue + decimalMinValue) / 2m
    let private decimalRange = decimalMaxValue - decimalMinValue
    let private decimalRangeModifier = (decimalMaxValue - decimalMinValue) / 2m

    /// Numerical representation of how a person/group sees another person/group.
    /// This is a the result of a computation and not a value that goes into the computation.
    type T =
        private | RelationshipScore of decimal
        static member (+) (s1: T, s2: T) : T =
            match s1, s2 with
            | RelationshipScore a, RelationshipScore b ->
                RelationshipScore (Math.Max(Math.Min((a + b), decimalMaxValue), decimalMinValue))
        static member (-) (s1: T, s2: T) : T =
            match s1, s2 with
            | RelationshipScore a, RelationshipScore b ->
                RelationshipScore (Math.Max(Math.Min((a - b), decimalMaxValue), decimalMinValue))
        static member (/) (s1: T, s2: T) : decimal =
            match s1, s2 with
            | RelationshipScore a, RelationshipScore b ->
                a / b
        static member (*) (d: decimal, s: T) : T =
            match s with RelationshipScore score -> RelationshipScore (d * score)
                
    let minValue = RelationshipScore decimalMinValue
    let maxValue = RelationshipScore decimalMaxValue
    let defaultValue = RelationshipScore decimalDefaultValue
    let defaultGroupValue = RelationshipScore (decimalMinValue + 0.6m * decimalRange)
        
    let createWithContinuation onError onSuccess (value: decimal) =
        if (value >= decimalMinValue && value <= decimalMaxValue) then onSuccess (RelationshipScore value)
        else onError value

    let createWithContinuationMany onError onSuccess (values: decimal seq) =
        createWithContinuation onError onSuccess (values |> Seq.sum)
        
    /// Creation of a RelationshipScore that will never fail by automatically clamping the value in the interval [minValue,maxValue]
    let createClamped (value: decimal) =
        let clamp value = if value < decimalMinValue then RelationshipScore decimalMinValue else RelationshipScore decimalMaxValue
        createWithContinuation clamp id value
    
    let apply f (RelationshipScore s) = f s
    
    let value (RelationshipScore s) = s
    
    let add (r1: T) (r2: T) = r1 + r2
    
    let subtract (r1: T) (r2: T) = r1 - r2
    
    /// Returns the score as a factor between -1 (maximum dislike) and 1 (maximum like)
    let asFactor (s: T) : decimal =
        match s with | RelationshipScore score -> (score - decimalMiddleValue) * decimalRangeModifier

    let factor (s: T) (factor: decimal) : T =
        match s with | RelationshipScore score -> createClamped ((score - decimalMiddleValue) * factor)
        
    let merge (scores: T list) : T =
        scores
        |> List.map (function RelationshipScore s -> s)
        |> List.average
        |> createClamped
        