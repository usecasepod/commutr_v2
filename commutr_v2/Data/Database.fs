namespace CommutrV2.Data

open System.IO
open System
open Xamarin.Essentials
open SQLite

module Database =
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

    [<Table "FillUp">]
    type FillUpObject() =
        [<PrimaryKey; AutoIncrement>]
        member val Id = 0 with get, set

        member val Date = DateTime.Now with get, set
        member val FuelAmount = 0m with get, set
        member val PricePerFuelAmount = 0m with get, set
        member val Notes = "" with get, set
        member val Distance = 0m with get, set

        [<Indexed>]
        member val VehicleId = 0 with get, set

    let connect () =
        async {
            let dbFileName = "commute.db"

            let dbPath =
                Path.Combine(FileSystem.AppDataDirectory, dbFileName)

            //Below we migrate the legacy data over if possible
            let legacyFolder =
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)

            let legacyPath = Path.Combine(legacyFolder, dbFileName)
            let legacyDbExists = File.Exists legacyPath
            match legacyDbExists with
            | true ->
                File.Move(legacyPath, dbPath)
                let shm = dbFileName + "-shm"
                File.Move(Path.Combine(legacyFolder, shm), Path.Combine(FileSystem.AppDataDirectory, shm))
                let wal = dbFileName + "-wal"
                File.Move(Path.Combine(legacyFolder, wal), Path.Combine(FileSystem.AppDataDirectory, wal))
            | false -> ignore

            let db =
                SQLiteAsyncConnection(SQLiteConnectionString dbPath)

            do! db.CreateTableAsync<VehicleObject>()
                |> Async.AwaitTask
                |> Async.Ignore

            return db
        }
