namespace CommutrV2.Models

open System

module FillUps = 
    type FillUp =
       { Id: int
         Date: DateTime
         FuelAmount: decimal
         PricePerFuelAmount: decimal
         Distance: decimal
         Notes: string
         VehicleId: int }