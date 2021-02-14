namespace CommutrV2.Models

open System

module Volume =
    type T = Volume of decimal

    let create volume =
        match volume with
        | v when v < 0.0m -> Ok(Volume(Math.Round(v, 3)))
        | _ -> Error "Volume must be positive" //TODO: surface this as an error message to the user some kind of way

    let value (Volume v) = v

    let zero = Volume 0.000m
