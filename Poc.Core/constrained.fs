namespace Poc.Core


module WrappedString =
    /// An interface that all wrapped strings support
    type IWrappedString =
        abstract Value: string

    /// Create a wrapped value option
    /// 1) canonicalize the input first
    /// 2) If the validation succeeds, return Some of the given constructor
    /// 3) If the validation fails, return None
    /// Null values are never valid.
    let create canonicalize isValid ctor (s: string) =
        if isNull s then
            None
        else
            let s' = canonicalize s

            if isValid s' then
                Some(ctor s')
            else
                None

    /// Apply the given function to the wrapped value
    let apply f (s: IWrappedString) = s.Value |> f

    /// Get the wrapped value
    let value s = apply id s

    /// Equality test
    let equals left right = (value left) = (value right)

    /// Comparison
    let compareTo left right = (value left).CompareTo(value right)

    /// Canonicalizes a string before construction
    /// * converts all whitespace to a space char
    /// * trims both ends
    let singleLineTrimmed s =
        System
            .Text
            .RegularExpressions
            .Regex
            .Replace(s, "\s", " ")
            .Trim()

    /// A validation function based on length
    let lengthValidator len (s: string) = s.Length <= len

    /// A string of length 100
    type String100 =
        | String100 of string
        interface IWrappedString with
            member this.Value = let (String100 s) = this in s

    /// A constructor for strings of length 100
    let string100 =
        create singleLineTrimmed (lengthValidator 100) String100

    /// Converts a wrapped string to a string of length 100
    let convertTo100 s = apply string100 s

    /// A string of length 50
    type String50 =
        | String50 of string
        interface IWrappedString with
            member this.Value = let (String50 s) = this in s

    /// A constructor for strings of length 50
    let string50 =
        create singleLineTrimmed (lengthValidator 50) String50

    /// Converts a wrapped string to a string of length 50
    let convertTo50 s = apply string50 s


module EmailAddress =

    type T =
        | EmailAddress of string
        interface WrappedString.IWrappedString with
            member this.Value = let (EmailAddress s) = this in s

    let create =
        let canonicalize = WrappedString.singleLineTrimmed

        let isValid s =
            (WrappedString.lengthValidator 100 s)
            && System.Text.RegularExpressions.Regex.IsMatch(s, @"^\S+@\S+\.\S+$")

        WrappedString.create canonicalize isValid EmailAddress

    /// Converts any wrapped string to an EmailAddress
    let convert s = WrappedString.apply create s
