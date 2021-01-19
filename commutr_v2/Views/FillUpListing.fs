namespace CommutrV2.Views

open CommutrV2.Models.FillUps
open CommutrV2.Models
open CommutrV2.Data.FillUpRepository
open Fabulous

module FillUpListing =
    type Model =
        { VehicleId: VehicleId.T
          FillUps: FillUp list }
        
    type ExternalMsg =
        | NoOp
        | NavigateToAdd
        | NavigateToUpdate of FillUp
        | NavigateToDetails of FillUp

    type Msg =
        | NewFillUpTapped
        | RemoveFillUp of FillUp
        | UpdateFillUp of FillUp
        | LoadFillUps
        | FillUpsLoaded of FillUp list
        | FillUpTapped of FillUp

    let loadAsync (vehicleId) =
        async {
            let! vehicles = loadFillUpsByVehicleId vehicleId
            return FillUpsLoaded vehicles
        }
        
    let deleteAsync (fillUp) =
        async {
            do! deleteFillUp fillUp
            let! msg = loadAsync (fillUp.VehicleId)
            return msg
        }

    let updateAsync (fillUp) =
        async {
            do! updateFillUp fillUp |> Async.Ignore
            let! msg = loadAsync (fillUp.VehicleId)
            return msg
        }
        
    let init (vehicleId) = { VehicleId = vehicleId; FillUps = [] }, Cmd.ofMsg LoadFillUps
