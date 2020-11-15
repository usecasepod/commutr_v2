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
          SelectedVehicle: Option<Vehicle>
          IsLoading: bool }

    type ExternalMsg =
        | NoOp
        | NavigateToAdd
        | NavigateToUpdate of Vehicle

    type Msg =
        | VehicleAdded of Vehicle
        | NewVehicleTapped
        | VehicleRemoved of Vehicle
        | VehicleModified of Vehicle
        | VehiclesLoaded of Vehicle list

    let loadAsync () = async {
        let! vehicles = loadAllVehicles ()
        return VehiclesLoaded vehicles
    }

    let init () =
        let m =
            { Vehicles = []
              SelectedVehicle = None
              IsLoading = true }
        m, Cmd.ofAsyncMsg (loadAsync ())

    let updateVehicles model vehicles =
        let m = { model with Vehicles = vehicles }
        m, Cmd.none, ExternalMsg.NoOp

    let update msg model =
        let addVehicle = fun v vehicles -> v :: vehicles
        let removeVehicle = fun (v: Vehicle) (vehicles: Vehicle list) -> vehicles |> List.filter (fun item -> item.Id <> v.Id)
        match msg with
        | VehicleAdded vehicle ->
            let updatedVehicles = addVehicle vehicle model.Vehicles
            updateVehicles model updatedVehicles
        | VehicleRemoved vehicle ->
            let updatedVehicles = removeVehicle vehicle model.Vehicles //TODO: remove from db
            updateVehicles model updatedVehicles
        | VehicleModified vehicle ->
            let updatedVehicles = addVehicle vehicle (removeVehicle vehicle model.Vehicles) //TODO: save modifications to db
            updateVehicles model updatedVehicles
        | NewVehicleTapped ->
            model, Cmd.none, ExternalMsg.NavigateToAdd
        | VehiclesLoaded vehicles ->
            let m = { model with Vehicles = vehicles; IsLoading = false }
            m, Cmd.none, ExternalMsg.NoOp

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
                                      command = fun () -> dispatch (VehicleRemoved itemModel)) ]),
                     leftItems =
                         View.SwipeItems
                             (items =
                                 [ View.SwipeItem
                                     (text = (if not itemModel.IsPrimary then "Primary" else "Not Primary"),
                                      backgroundColor =
                                          (if not itemModel.IsPrimary then
                                              AppColors.mandarin
                                           else
                                               AppColors.silverSandMediumDark),
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
