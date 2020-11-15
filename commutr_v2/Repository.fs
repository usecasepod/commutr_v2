namespace CommutrV2

open CommutrV2.Models
open System.IO
open System
open Xamarin.Essentials
open SQLite

module Repository =
    [<Table "Vehicle">]
    type VehicleObject() =
        [<PrimaryKey; AutoIncrement>]
        member val Id = 0 with get, set
        member val Make = "" with get, set
        member val Model = "" with get, set
        member val Year = 0 with get, set
        member val Odometer = 0m with get, set
        member val Notes = "" with get, set
        member val IsPrimary = false with get, set

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

    let convertToModel (obj: VehicleObject) : Vehicle =
        { Id = obj.Id
          Make = obj.Make
          Model = obj.Model
          Year = obj.Year
          Odometer = obj.Odometer
          Notes = obj.Notes
          IsPrimary = obj.IsPrimary }

    let connect () = async {
        let dbFileName = "commute.db"
        let dbPath = Path.Combine(FileSystem.AppDataDirectory, dbFileName)

        //Below we migrate the legacy data over if possible
        let legacyFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
        let legacyPath = Path.Combine(legacyFolder, dbFileName)
        let legacyDbExists = File.Exists legacyPath
        match legacyDbExists with
        | true ->
            File.Move (legacyPath, dbPath)
            let shm = dbFileName + "-shm"
            File.Move (Path.Combine(legacyFolder, shm), Path.Combine(FileSystem.AppDataDirectory, shm))
            let wal = dbFileName + "-wal"
            File.Move (Path.Combine(legacyFolder, wal), Path.Combine(FileSystem.AppDataDirectory, wal))
        | false -> ignore
        
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

    let updateVehicle (vehicle: Vehicle) = async {
        let! database = connect()
        let obj = convertToObject vehicle
        do! database.UpdateAsync(obj) |> Async.AwaitTask |> Async.Ignore
        return vehicle
    }

    let deleteVehicle (vehicle: Vehicle) = async {
        let! database = connect()
        let obj = convertToObject vehicle
        do! database.DeleteAsync(obj) |> Async.AwaitTask |> Async.Ignore
    }
        