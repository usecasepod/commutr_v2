namespace CommutrV2.Models

open System

module FillUps =
    type FillUp =
        { Id: FillUpId.T
          Date: DateTime
          FuelAmount: Volume.T
          PricePerFuelAmount: decimal
          Distance: Distance.T
          Notes: string
          VehicleId: VehicleId.T }

    let calculateTotal fuelAmount pricePerFuelAmount =
        match (fuelAmount, pricePerFuelAmount) with
        | (Some fuel, Some price) -> Math.Round(Volume.value fuel * price, 2)
        | _ -> 0.0m
