namespace CommutrV2.Models

open System

module Distance =
    type T = Distance of decimal

    let create distance =
        match distance with
        | d when d > 0.0m -> Ok(Distance(Math.Round(d, 2)))
        | _ -> Error "Distance must be positive" //TODO: surface this as an error message to the user some kind of way

    let value (Distance d) = d

    let zero = Distance 0.0m
