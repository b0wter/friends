namespace Friends.Lib

open System

module SignificanceInGroup =
    let decimalMinValue = 0m
    let decimalMaxValue = 1m
    let decimalDefaultValue = 0.25m
    
    /// Describes how well known a person/group is in a given group
    /// This value is mostly relevant to compute the propagation of renown/relationship score
    type T =
        private | SignificanceInGroup of decimal
        static member (+) (s1: T, s2: T) : T =
            match s1, s2 with
            | SignificanceInGroup a, SignificanceInGroup b ->
                SignificanceInGroup (Math.Max(Math.Min((a + b), decimalMaxValue), decimalMinValue))
        static member (-) (s1: T, s2: T) : T =
            match s1, s2 with
            | SignificanceInGroup a, SignificanceInGroup b ->
                SignificanceInGroup (Math.Max(Math.Min((a - b), decimalMaxValue), decimalMinValue))
                
    let minValue = SignificanceInGroup decimalMinValue
    let maxValue = SignificanceInGroup decimalMaxValue
    let defaultValue = SignificanceInGroup decimalDefaultValue
        
    let createWithContinuation onError onSuccess (value: decimal) =
        if (value >= decimalMinValue && value <= decimalMaxValue) then onSuccess (SignificanceInGroup value)
        else onError value
        
    /// Creation of a SignificanceInGroup that will never fail by automatically clamping the value in the interval [minValue,maxValue]
    let createClamped (value: decimal) =
        let clamp value = if value < decimalMinValue then SignificanceInGroup decimalMinValue else SignificanceInGroup decimalMaxValue
        createWithContinuation clamp id value

    let apply f (SignificanceInGroup s) = f s
    
    let value (SignificanceInGroup s) = s
    
    let add (r1: T) (r2: T) = r1 + r2
    
    let subtract (r1: T) (r2: T) = r1 - r2
    

