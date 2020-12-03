namespace CommutrV2.Models

module Vehicles =
    type Vehicle =
        { Id: int
          Make: string
          Model: string
          Year: int
          Odometer: decimal
          Notes: string
          IsPrimary: bool }

    let formattedName vehicle =
        sprintf "%i %s %s" vehicle.Year vehicle.Make vehicle.Model
