namespace CommutrV2.Data

open CommutrV2.Models.Vehicles
open CommutrV2.Data.Database

module VehicleRepository =
    let convertToObject (item: Vehicle) =
        let obj = VehicleObject()
        obj.Id <- item.Id
        obj.Make <- item.Make
        obj.Model <- item.Model
        obj.Year <- item.Year
        obj.Odometer <- item.Odometer
        obj.Notes <- item.Notes
        obj.IsPrimary <- item.IsPrimary
        obj

    let convertToModel (obj: VehicleObject): Vehicle =
        { Id = obj.Id
          Make = obj.Make
          Model = obj.Model
          Year = obj.Year
          Odometer = obj.Odometer
          Notes = obj.Notes
          IsPrimary = obj.IsPrimary }

    let loadAllVehicles () =
        async {
            let! database = connect ()

            let! objs =
                database.Table<VehicleObject>().ToListAsync()
                |> Async.AwaitTask

            return objs |> Seq.toList |> List.map convertToModel
        }

    let insertVehicle (vehicle) =
        async {
            let! database = connect ()
            let obj = convertToObject vehicle

            do! database.InsertAsync(obj)
                |> Async.AwaitTask
                |> Async.Ignore

            let! rowIdObj =
                database.ExecuteScalarAsync("select last_insert_rowid()", [||])
                |> Async.AwaitTask

            let rowId = rowIdObj |> int

            return { vehicle with Id = rowId }
        }

    let updateVehicle (vehicle) =
        async {
            let! database = connect ()
            let obj = convertToObject vehicle

            do! database.UpdateAsync(obj)
                |> Async.AwaitTask
                |> Async.Ignore

            return vehicle
        }

    let deleteVehicle (vehicle) =
        async {
            let! database = connect ()
            let obj = convertToObject vehicle

            do! database.DeleteAsync(obj)
                |> Async.AwaitTask
                |> Async.Ignore
        }
