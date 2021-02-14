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
        let total =
            Volume.value fuelAmount * pricePerFuelAmount

        Math.Round(total, 2)
