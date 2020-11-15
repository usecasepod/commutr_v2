namespace CommutrV2.Views

open CommutrV2
open CommutrV2.Models
open CommutrV2.Components
open CommutrV2.Repository
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module VehicleListing =
    type Model =
        { Vehicles: Vehicle list
          IsLoading: bool }

    type ExternalMsg =
        | NoOp
        | NavigateToAdd
        | NavigateToUpdate of Vehicle

    type Msg =
        | NewVehicleTapped
        | RemoveVehicle of Vehicle
        | UpdateVehicle of Vehicle
        | VehicleModified of Vehicle
        | LoadVehicles
        | VehiclesLoaded of Vehicle list

    let loadAsync () = async {
        let! vehicles = loadAllVehicles ()
        return VehiclesLoaded vehicles
    }

    let deleteAsync (vehicle) = async {
       do! deleteVehicle vehicle
       let! msg = loadAsync ()
       return msg
    }

    let updateAsync (vehicle) = async {
        do! updateVehicle vehicle |> Async.Ignore
        let! msg = loadAsync()
        return msg
    }

    let initModel =
        { Vehicles = []
          IsLoading = true }

    let init () =
        initModel, Cmd.ofMsg LoadVehicles

    let updateVehicles model vehicles =
        let m = { model with Vehicles = vehicles }
        m, Cmd.none, ExternalMsg.NoOp

    let update msg model =
        let addVehicle = fun v vehicles -> v :: vehicles
        let removeVehicle = fun (v: Vehicle) (vehicles: Vehicle list) -> vehicles |> List.filter (fun item -> item.Id <> v.Id)
        match msg with
        | LoadVehicles ->
            let cmd = Cmd.ofAsyncMsg (loadAsync())
            initModel, cmd, ExternalMsg.NoOp
        | VehiclesLoaded vehicles ->
            let m = { model with Vehicles = vehicles; IsLoading = false }
            m, Cmd.none, ExternalMsg.NoOp
        | RemoveVehicle vehicle ->
            let cmd = Cmd.ofAsyncMsg(deleteAsync vehicle)
            model, cmd, ExternalMsg.NoOp
        | VehicleModified vehicle ->
            let updatedVehicles = addVehicle vehicle (removeVehicle vehicle model.Vehicles) //TODO: save modifications to db
            updateVehicles model updatedVehicles
        | NewVehicleTapped ->
            model, Cmd.none, ExternalMsg.NavigateToAdd
        | UpdateVehicle vehicle ->
            model, Cmd.none, ExternalMsg.NavigateToUpdate vehicle

    let view model dispatch =
        let items =
            model.Vehicles
            |> List.map (fun itemModel ->
                View.SwipeView
                    (backgroundColor = AppColors.silverSandLight,
                     rightItems =
                         View.SwipeItems
                             (items =
                                 [ View.SwipeItem
                                     (text = "Delete",
                                      backgroundColor = Color.Red,
                                      command = fun () -> dispatch (RemoveVehicle itemModel))
                                   View.SwipeItem
                                     (text = "Edit",
                                      backgroundColor = AppColors.silverSandMediumDark,
                                      command = fun () -> dispatch (UpdateVehicle itemModel))]),
                     leftItems =
                         View.SwipeItems
                             (items =
                                 [ View.SwipeItem
                                     (text =
                                          (match itemModel.IsPrimary with
                                          | false -> "Primary"
                                          | true -> "Not Primary"),
                                      backgroundColor =
                                          (match itemModel.IsPrimary with
                                          | false -> AppColors.mandarin
                                          | true -> AppColors.silverSandMediumDark),
                                      command =
                                          fun () ->
                                              dispatch
                                                  (VehicleModified { itemModel with IsPrimary = not itemModel.IsPrimary})) ]),
                     content = VehicleCell.view itemModel))
        let emptyView =
            match model.IsLoading with
            | true ->
                View.Label
                    (text = "Loading Vehicles!",
                     horizontalTextAlignment = TextAlignment.Center,
                     verticalTextAlignment = TextAlignment.Center,
                     fontAttributes = FontAttributes.Bold,
                     textColor = AppColors.cinereous)
            | false ->
                View.StackLayout
                    (spacing = 5.0,
                     backgroundColor = AppColors.silverSandLight,
                     padding = Thickness 25.0,
                     children =
                         [ View.Label
                             (text = "You don't have any vehicles!",
                              horizontalTextAlignment = TextAlignment.Center,
                              verticalTextAlignment = TextAlignment.Center,
                              fontAttributes = FontAttributes.Bold,
                              textColor = AppColors.cinereous)
                           View.Button
                               (text = "Add a Vehicle",
                                backgroundColor = AppColors.cinereous,
                                textColor = AppColors.ghostWhite,
                                command = fun () -> dispatch NewVehicleTapped) ])


        View.ContentPage
            (View.CollectionView
                (items,
                 emptyView = emptyView),
            backgroundColor = AppColors.silverSandLight,
            title = "Vehicles")
