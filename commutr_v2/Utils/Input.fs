namespace CommutrV2.Utils

open CommutrV2.Models
open System

module Input =
    let distanceToString d =
        match Distance.value d with
        | 0m -> ""
        | d -> d.ToString()

    let stringToDistance str =
        match str with
        | "" -> Some(Distance.T.Distance 0.0m)
        | _ ->
            try
                str
                |> decimal
                |> Distance.create
                |> fun result ->
                    match result with
                    | Ok o -> Some o
                    | _ -> None
            with :? FormatException -> None

    let yearToString year =
        match Year.value year with
        | 0 -> ""
        | y -> y.ToString()

    let stringToYear str =
        match str with
        | "" -> 0 |> Year.create |> Some
        | _ ->
            try
                str |> int |> Year.create |> Some
            with :? FormatException -> None
