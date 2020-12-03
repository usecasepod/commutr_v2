﻿namespace CommutrV2.Data

open CommutrV2.Models.FillUps
open CommutrV2.Data.Database
open System.Linq

module FillUpRepository =
    let convertToObject (item: FillUp) =
        let obj = FillUpObject()
        obj.Id <- item.Id
        obj.Date <- item.Date
        obj.FuelAmount <- item.FuelAmount
        obj.PricePerFuelAmount <- item.PricePerFuelAmount
        obj.Notes <- item.Notes
        obj.Distance <- item.Distance
        obj.VehicleId <- item.VehicleId
        obj

    let convertToModel (obj: FillUpObject): FillUp =
        { Id = obj.Id
          Date = obj.Date
          FuelAmount = obj.FuelAmount
          PricePerFuelAmount = obj.PricePerFuelAmount
          Notes = obj.Notes
          Distance = obj.Distance
          VehicleId = obj.VehicleId }

    let loadFillUpsByVehicleId (vehicleId) =
        async {
            let! database = connect ()

            let! objs =
                database.Table<FillUpObject>().Where(fun x -> x.VehicleId = vehicleId).ToListAsync()
                |> Async.AwaitTask

            return objs |> Seq.toList |> List.map convertToModel
        }

    let insertFillUp (fillUp) =
        async {
            let! database = connect ()
            let obj = convertToObject fillUp

            do! database.InsertAsync(obj)
                |> Async.AwaitTask
                |> Async.Ignore

            let! rowIdObj =
                database.ExecuteScalarAsync("select last_insert_rowid()", [||])
                |> Async.AwaitTask

            let rowId = rowIdObj |> int
            return { fillUp with Id = rowId }
        }

    let updateFillUp (fillUp) =
        async {
            let! database = connect ()
            let obj = convertToObject fillUp

            do! database.UpdateAsync(obj)
                |> Async.AwaitTask
                |> Async.Ignore

            return fillUp
        }

    let deleteFillUp (fillUp) =
        async {
            let! database = connect ()
            let obj = convertToObject fillUp

            do! database.DeleteAsync(obj)
                |> Async.AwaitTask
                |> Async.Ignore
        }