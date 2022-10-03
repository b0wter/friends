namespace Friends.Lib

open System

module RelationshipScore =
    let decimalMinValue = 0m
    let decimalMaxValue = 2m
    let decimalDefaultValue = 1m

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
                
    let minValue = RelationshipScore decimalMinValue
    let maxValue = RelationshipScore decimalMaxValue
    let defaultValue = RelationshipScore decimalDefaultValue
        
    let createWithContinuation onError onSuccess (value: decimal) =
        if (value >= decimalMinValue && value <= decimalMaxValue) then onSuccess (RelationshipScore value)
        else onError value
        
    /// Creation of a RelationshipScore that will never fail by automatically clamping the value in the interval [minValue,maxValue]
    let createClamped (value: decimal) =
        let clamp value = if value < decimalMinValue then RelationshipScore decimalMinValue else RelationshipScore decimalMaxValue
        createWithContinuation clamp id value

    let apply f (RelationshipScore s) = f s
    
    let value (RelationshipScore s) = s
    
    let add (r1: T) (r2: T) = r1 + r2
    
    let subtract (r1: T) (r2: T) = r1 - r2