namespace CommutrV2.Models

module Year =
    type T = Year of int
    let create y = Year y //TODO: consider some validation here
    let value (Year y) = y
