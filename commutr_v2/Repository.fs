namespace CommutrV2

open CommutrV2.Models
open System.IO
open Xamarin.Essentials
open SQLite

module Repository =
    type VehicleObject() =
        [<PrimaryKey; AutoIncrement>]
        member val Id = 0 with get, set
        member val Make = "" with get, set
        member val Model = "" with get, set
        member val Year = 0 with get, set
        member val IsPrimary = false with get, set

    let convertToObject (item: Vehicle) =
        let obj = VehicleObject()
        obj.Id <- item.Id
        obj.Make <- item.Make
        obj.Model <- item.Model
        obj.Year <- item.Year
        obj.IsPrimary <- item.IsPrimary
        obj

    let convertToModel (obj: VehicleObject) : Vehicle =
        { Id = obj.Id
          Make = obj.Make
          Model = obj.Model
          Year = obj.Year
          IsPrimary = obj.IsPrimary }

    let connect () = async {
        let dbPath = Path.Combine(FileSystem.AppDataDirectory, "commutr.db3")
        let db = SQLiteAsyncConnection(SQLiteConnectionString dbPath)
        do! db.CreateTableAsync<VehicleObject>() |> Async.AwaitTask |> Async.Ignore
        return db
    }

    let loadAllVehicles () = async {
        let! database = connect ()
        let! objs = database.Table<VehicleObject>().ToListAsync() |> Async.AwaitTask
        return objs |> Seq.toList |> List.map convertToModel
    }

    let insertVehicle (vehicle: Vehicle) = async {
        let! database = connect()
        let obj = convertToObject vehicle
        do! database.InsertAsync(obj) |> Async.AwaitTask |> Async.Ignore
        let! rowIdObj = database.ExecuteScalarAsync("select last_insert_rowid()", [||]) |> Async.AwaitTask
        let rowId = rowIdObj |> int
        return { vehicle with Id = rowId }
    }
        