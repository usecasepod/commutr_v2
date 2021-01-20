namespace CommutrV2.Views

open CommutrV2.Models.FillUps
open CommutrV2.Models
open CommutrV2.Data.FillUpRepository
open CommutrV2.Utils.Loading
open Fabulous

module FillUpListing =

    type Model =
        { VehicleId: VehicleId.T
          FillUpLoadable: FillUp Loadable }

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

    let init (vehicleId) =
        { VehicleId = vehicleId
          FillUpLoadable = Loading },
        Cmd.ofMsg LoadFillUps

    let update msg model =
        match msg with
        | LoadFillUps ->
            let cmd =
                Cmd.ofAsyncMsg (loadAsync (model.VehicleId))

            let m = { model with FillUpLoadable = Loading }
            m, cmd, ExternalMsg.NoOp
        | FillUpsLoaded fillUps ->
            let m =
                { model with
                      FillUpLoadable = Loaded fillUps }

            m, Cmd.none, ExternalMsg.NoOp

// let view model dispatch =
//      let content = match model.FillUpLoadable with
//                    | Loading ->
//                         View.Label
//                             (text = "Loading...",
//                              horizontalTextAlignment = TextAlignment.Center,
//                              verticalTextAlignment = TextAlignment.Center,
//                              fontAttributes = FontAttributes.Bold,
//                              textColor = AppColors.cinereous)
//                    | Loaded fillUps ->
//                        let items = fillUps |>
