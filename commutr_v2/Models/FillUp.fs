namespace CommutrV2.Models

open System

module FillUps =
    type FillUp =
        { Id: FillUpId.T
          Date: DateTime
          FuelAmount: decimal
          PricePerFuelAmount: decimal
          Distance: Distance.T
          Notes: string
          VehicleId: VehicleId.T }
