namespace CommutrV2.Models

module FillUpId =
    type T = FillUpId of int
    let create id = FillUpId id
    let value (FillUpId id) = id
