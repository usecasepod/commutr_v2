namespace CommutrV2.Views

open CommutrV2
open CommutrV2.Models.Vehicles
open CommutrV2.Components
open CommutrV2.Data.VehicleRepository
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module VehicleListing =
    type Model =
        { Vehicles: Vehicle list
          ShouldNavigate: bool
          IsLoading: bool }

    type ExternalMsg =
        | NoOp
        | NavigateToAdd
        | NavigateToUpdate of Vehicle
        | NavigateToDetails of Vehicle //TODO: create details (tabbed) page and handle navigation

    type Msg =
        | NewVehicleTapped
        | RemoveVehicle of Vehicle
        | UpdateVehicle of Vehicle
        | VehicleModified of Vehicle
        | LoadVehicles
        | VehiclesLoaded of Vehicle list
        | VehicleTapped of Vehicle

    let loadAsync () =
        async {
            let! vehicles = loadAllVehicles ()
            return VehiclesLoaded vehicles
        }

    let deleteAsync (vehicle) =
        async {
            do! deleteVehicle vehicle
            let! msg = loadAsync ()
            return msg
        }

    let updateAsync (vehicle) =
        async {
            do! updateVehicle vehicle |> Async.Ignore
            let! msg = loadAsync ()
            return msg
        }

    let initModel =
        { Vehicles = []
          ShouldNavigate = true
          IsLoading = true }

    let init () = initModel, Cmd.ofMsg LoadVehicles

    let updateVehicles model vehicles =
        let m = { model with Vehicles = vehicles }
        m, Cmd.none, ExternalMsg.NoOp

    let update msg model =
        let addVehicle = fun v vehicles -> v :: vehicles

        let removeVehicle =
            fun (v: Vehicle) (vehicles: Vehicle list) ->
                vehicles
                |> List.filter (fun item -> item.Id <> v.Id)

        match msg with
        | LoadVehicles ->
            let cmd = Cmd.ofAsyncMsg (loadAsync ())
            { model with
                  Vehicles = []
                  IsLoading = true },
            cmd,
            ExternalMsg.NoOp
        | VehiclesLoaded vehicles ->
            let externalMsg =
                match model.ShouldNavigate with
                | true ->
                    let vehOption =
                        vehicles |> List.tryFind (fun x -> x.IsPrimary)

                    match vehOption with
                    | None -> ExternalMsg.NoOp
                    | Some vehicle -> ExternalMsg.NavigateToDetails vehicle
                | false -> ExternalMsg.NoOp

            let m =
                { model with
                      Vehicles = vehicles
                      ShouldNavigate = false
                      IsLoading = false }

            m, Cmd.none, externalMsg
        | RemoveVehicle vehicle ->
            let cmd = Cmd.ofAsyncMsg (deleteAsync vehicle)
            model, cmd, ExternalMsg.NoOp
        | VehicleModified vehicle ->
            let updatedVehicles =
                addVehicle vehicle (removeVehicle vehicle model.Vehicles) //TODO: save modifications to db

            updateVehicles model updatedVehicles
        | NewVehicleTapped -> model, Cmd.none, ExternalMsg.NavigateToAdd
        | UpdateVehicle vehicle -> model, Cmd.none, ExternalMsg.NavigateToUpdate vehicle
        | VehicleTapped vehicle -> model, Cmd.none, ExternalMsg.NavigateToDetails vehicle

    let view model dispatch =
        let items =
            model.Vehicles
            |> List.map (fun itemModel ->
                View.SwipeView
                    (backgroundColor = AppColors.silverSandLight,
                     gestureRecognizers =
                         [ View.TapGestureRecognizer(command = fun () -> dispatch (VehicleTapped itemModel)) ],
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
                                        command = fun () -> dispatch (UpdateVehicle itemModel)) ]),
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
                                                  (VehicleModified
                                                      { itemModel with
                                                            IsPrimary = not itemModel.IsPrimary })) ]),
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
            (View.AbsoluteLayout
                ([ View.CollectionView(items, emptyView = emptyView, selectionMode = SelectionMode.Single)
                   View.Button(text = "+",
                               fontSize = FontSize.fromValue 24.0,
                               backgroundColor = AppColors.mandarin,
                               textColor = AppColors.ghostWhite,
                               command = (fun () -> dispatch NewVehicleTapped),
                               padding = Thickness 10.0).WidthRequest(60.0).HeightRequest(60.0).ButtonCornerRadius(30)
                       .LayoutFlags(AbsoluteLayoutFlags.PositionProportional)
                       .LayoutBounds(Rectangle(0.90, 1.0, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize)) ]),
             backgroundColor = AppColors.silverSandLight,
             title = "Vehicles")
