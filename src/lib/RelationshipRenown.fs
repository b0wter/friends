namespace Friends.Lib

open System

module RelationshipRenown =
    let decimalMinValue = 0m
    let decimalMaxValue = 1m
    let decimalDefaultValue = 0m
    let private decimalMiddleValue = (decimalMaxValue + decimalMinValue) / 2m
    let private decimalRange = decimalMaxValue - decimalMinValue
    let private decimalRangeModifier = (decimalMaxValue - decimalMinValue) / 2m
    
    /// Describes how well known a person/group is to another person/group.
    /// This value is mostly relevant to check if a person or member of a group knows the player.
    type T =
        private | RelationshipRenown of decimal
        static member (+) (s1: T, s2: T) : T =
            match s1, s2 with
            | RelationshipRenown a, RelationshipRenown b ->
                RelationshipRenown (Math.Max(Math.Min((a + b), decimalMaxValue), decimalMinValue))
        static member (-) (s1: T, s2: T) : T =
            match s1, s2 with
            | RelationshipRenown a, RelationshipRenown b ->
                RelationshipRenown (Math.Max(Math.Min((a - b), decimalMaxValue), decimalMinValue))
        static member (*) (s1: T, s2: T) : T =
            match s1, s2 with
            | RelationshipRenown a, RelationshipRenown b ->
                RelationshipRenown (Math.Max(Math.Min((a * b), decimalMaxValue), decimalMinValue))
                
    let minValue = RelationshipRenown decimalMinValue
    let maxValue = RelationshipRenown decimalMaxValue
    let defaultValue = RelationshipRenown decimalDefaultValue
        
    let createWithContinuation onError onSuccess (value: decimal) =
        if (value >= decimalMinValue && value <= decimalMaxValue) then onSuccess (RelationshipRenown value)
        else onError value
        
    /// Creation of a RelationshipRenown that will never fail by automatically clamping the value in the interval [minValue,maxValue]
    let createClamped (value: decimal) =
        let clamp value = if value < decimalMinValue then RelationshipRenown decimalMinValue else RelationshipRenown decimalMaxValue
        createWithContinuation clamp id value

    let apply f (RelationshipRenown s) = f s
    
    let value (RelationshipRenown s) = s
    
    let add (r1: T) (r2: T) = r1 + r2
    
    let subtract (r1: T) (r2: T) = r1 - r2
    
    /// Returns the renown as a factor between 0 (totally unknown) and 1 (very well known)
    let asFactor (r: T) : decimal =
        match r with | RelationshipRenown renown -> (renown - decimalMinValue) / decimalRange
