namespace Friends.Lib.Tests

open Friends.Lib
open Xunit
open FsUnit.Xunit

module RelationshipRenown =

    [<Theory>]
    [<InlineData( 00,  00)>]
    [<InlineData( 10,  10)>]
    [<InlineData( 20,  20)>]
    [<InlineData( 25,  25)>]
    [<InlineData( 50,  50)>]
    [<InlineData( 60,  60)>]
    [<InlineData( 70,  70)>]
    [<InlineData( 75,  75)>]
    [<InlineData(100, 100)>]
    let ``Computing a normalized factor from a relationship score returns correct value`` (i1, i2) =
        let decimalScore = i1 / 100m
        let expectedFactor = i2 / 100m
        let score = RelationshipRenown.createClamped decimalScore
        let asFactor = score |> RelationshipRenown.asFactor
        asFactor |> should equal expectedFactor
