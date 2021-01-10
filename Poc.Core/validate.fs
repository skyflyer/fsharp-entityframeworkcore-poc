namespace Poc.Core

open System

module Validate =

    let ensureNonNegative x =
        match x with
        | x when x >= 0m -> x
        | x when x < 0m -> invalidArg "value" "must not be negative"
        | _ -> invalidArg "value" "Never happens" // TODO: why?

    let ensureString50 s =
        if String.IsNullOrEmpty(s) then
            invalidArg "value" "string must not be null"
        else if s.Length > 50 then
            invalidArg "value" "string must not be longer than 50 chars"
        else s