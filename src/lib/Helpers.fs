namespace Friends.Lib

open System.Runtime.CompilerServices

module Helpers =

    type System.Collections.Generic.Dictionary<'a, 'b> with
        member this.GetWith(key: 'a, fallback: 'b) : 'b =
            if this.ContainsKey(key) then this[key]
            else fallback

