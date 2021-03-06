namespace CommutrV2.Models

module Vehicles =
    type Vehicle =
        { Id: VehicleId.T
          Make: string
          Model: string
          Year: Year.T
          Odometer: Distance.T
          Notes: string
          IsPrimary: bool }

    let formattedName vehicle =
        let year = Year.value vehicle.Year
        $"{year} {vehicle.Make} {vehicle.Model}"
