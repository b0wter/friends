namespace Friends.Lib.Tests

open Xunit
open FsUnit.Xunit

open Friends.Lib

module RelationshipScore =
    let validScore = (RelationshipScore.decimalMinValue + RelationshipScore.decimalMaxValue) / 2m
    let tooLargeValue = RelationshipScore.decimalMaxValue + 0.1m
    let tooSmallValue = RelationshipScore.decimalMinValue - 0.1m
    
    [<Fact>]
    let ``Trying to create a score in the valid interval calls the onSuccess continuation`` () =
        RelationshipScore.createWithContinuation
            (fun _ -> Assert.True(false, "The continuation for the error case was called"))
            (fun _ -> Assert.True true)

    [<Fact>]
    let ``Trying to create a score above the upper limit of the interval calls the onError continuation`` () =
        RelationshipScore.createWithContinuation
            (fun _ -> Assert.True true)
            (fun _ -> Assert.True(false, "The continuation for the success case was called"))
            tooLargeValue

    [<Fact>]
    let ``Trying to create a score below the lower limit of the interval calls the onError continuation`` () =
        RelationshipScore.createWithContinuation
            (fun _ -> Assert.True true)
            (fun _ -> Assert.True(false, "The continuation for the success case was called"))
            tooSmallValue
            
    [<Fact>]
    let ``Trying to create a clamped score in the valid rage returns the given value`` () =
        let result = RelationshipScore.createClamped validScore
        result |> RelationshipScore.value |> should equal validScore

    [<Fact>]
    let ``Trying to create a clamped score with a value less than 0 returns a value of 0`` () =
        let result = RelationshipScore.createClamped tooSmallValue
        result |> RelationshipScore.value |> should equal RelationshipScore.decimalMinValue
            
    [<Fact>]
    let ``Trying to create a clamped score with a value greater than 1 returns a value of 1`` () =
        let result = RelationshipScore.createClamped tooLargeValue
        result |> RelationshipScore.value |> should equal RelationshipScore.decimalMaxValue

    [<Fact>]
    let ``Adding two scores returns the summed score`` () =
        let result = (RelationshipScore.createClamped 0.25m) + (RelationshipScore.createClamped 0.25m)
        result |> RelationshipScore.value |> should equal 0.5m
        
    [<Fact>]
    let ``Adding two scores whose sum is larger then the max value returns the max value`` () =
        let result = (RelationshipScore.createClamped RelationshipScore.decimalMaxValue) + (RelationshipScore.createClamped RelationshipScore.decimalMaxValue)
        result |> RelationshipScore.value |> should equal RelationshipScore.decimalMaxValue
    
    [<Fact>]
    let ``Adding two scores whose sum is less then the min value returns the min value`` () =
        let result = (RelationshipScore.createClamped RelationshipScore.decimalMinValue) + (RelationshipScore.createClamped -RelationshipScore.decimalMaxValue)
        result |> RelationshipScore.value |> should equal RelationshipScore.decimalMinValue
    
    [<Fact>]
    let ``Subtracting two scores returns the difference`` () =
        let result = (RelationshipScore.createClamped 0.5m) - (RelationshipScore.createClamped 0.25m)
        result |> RelationshipScore.value |> should equal 0.25m
        
    [<Fact>]
    let ``Subtracting two scores whose difference is larger then the max value returns the max value`` () =
        let result = (RelationshipScore.createClamped RelationshipScore.decimalMaxValue) - (RelationshipScore.createClamped -RelationshipScore.decimalMaxValue)
        result |> RelationshipScore.value |> should equal RelationshipScore.decimalMaxValue
    
    [<Fact>]
    let ``Subtracting two scores whose difference is less then the min value returns the min value`` () =
        let result = (RelationshipScore.createClamped RelationshipScore.decimalMinValue) - (RelationshipScore.createClamped -RelationshipScore.decimalMaxValue)
        result |> RelationshipScore.value |> should equal RelationshipScore.decimalMinValue

    [<Fact>]
    let ``Checking whether a large score is larger using the > operator returns true`` () =
        let result = (RelationshipScore.createClamped 0.5m) > (RelationshipScore.createClamped 0.25m)
        result |> should be True

    [<Fact>]
    let ``Checking whether a smaller score is larger using the > operator returns false`` () =
        let result = (RelationshipScore.createClamped 0.25m) > (RelationshipScore.createClamped 0.5m)
        result |> should be False

    [<Fact>]
    let ``Checking whether a large score is smaller using the < operator returns true`` () =
        let result = (RelationshipScore.createClamped 0.5m) < (RelationshipScore.createClamped 0.25m)
        result |> should be False

    [<Fact>]
    let ``Checking whether a smaller score is smaller using the < operator returns true`` () =
        let result = (RelationshipScore.createClamped 0.25m) < (RelationshipScore.createClamped 0.5m)
        result |> should be True

    [<Theory>]
    [<InlineData( 00, -100)>]
    [<InlineData( 10, -90)>]
    [<InlineData( 20, -80)>]
    [<InlineData( 25, -75)>]
    [<InlineData( 50, -50)>]
    [<InlineData( 60, -40)>]
    [<InlineData( 70, -30)>]
    [<InlineData( 75, -25)>]
    [<InlineData(100,   0)>]
    [<InlineData(110,  10)>]
    [<InlineData(120,  20)>]
    [<InlineData(125,  25)>]
    [<InlineData(150,  50)>]
    [<InlineData(160,  60)>]
    [<InlineData(170,  70)>]
    [<InlineData(175,  75)>]
    [<InlineData(200, 100)>]
    let ``Computing a normalized factor from a relationship score returns correct value`` (i1, i2) =
        let decimalScore = i1 / 100m
        let expectedFactor = i2 / 100m
        let score = RelationshipScore.createClamped decimalScore
        let asFactor = score |> RelationshipScore.asFactor
        asFactor |> should equal expectedFactor