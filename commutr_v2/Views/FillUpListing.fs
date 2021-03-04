namespace CommutrV2.Views

open CommutrV2
open CommutrV2.Components
open CommutrV2.Models.FillUps
open CommutrV2.Models.Vehicles
open CommutrV2.Models
open CommutrV2.Data.FillUpRepository
open CommutrV2.Utils.Loading
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms


module FillUpListing =
    type Model =
        { Vehicle: Vehicle
          FillUps: FillUp Loadable }

    type ExternalMsg =
        | NoOp
        | NavigateToAdd
        | NavigateToUpdate of FillUp

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

    let init (vehicle) =
        { Vehicle = vehicle; FillUps = Loading }, Cmd.ofMsg LoadFillUps

    let update msg model =
        match msg with
        | LoadFillUps ->
            let cmd =
                Cmd.ofAsyncMsg (loadAsync (model.Vehicle.Id))

            let m = { model with FillUps = Loading }
            m, cmd, ExternalMsg.NoOp
        | FillUpsLoaded fillUps ->
            let m = { model with FillUps = Loaded fillUps }

            m, Cmd.none, ExternalMsg.NoOp
        | NewFillUpTapped -> model, Cmd.none, ExternalMsg.NavigateToAdd
        | UpdateFillUp fillUp -> model, Cmd.none, ExternalMsg.NavigateToUpdate fillUp

    let view model dispatch =
        let content =
            match model.FillUps with
            | Loading ->
                View.Label(
                    text = "Loading...",
                    horizontalTextAlignment = TextAlignment.Center,
                    verticalTextAlignment = TextAlignment.Center,
                    fontAttributes = FontAttributes.Bold,
                    textColor = AppColors.cinereous
                )
            | Loaded fillUps ->
                let emptyView =
                    View.StackLayout(
                        spacing = 5.0,
                        backgroundColor = AppColors.silverSandLight,
                        padding = Thickness 25.0,
                        children =
                            [ View.Label(
                                text = "You don't have any fill ups recorded yet!",
                                horizontalTextAlignment = TextAlignment.Center,
                                verticalTextAlignment = TextAlignment.Center,
                                fontAttributes = FontAttributes.Bold,
                                textColor = AppColors.cinereous
                              )
                              View.Button(
                                  text = "Record a Fill Up",
                                  backgroundColor = AppColors.cinereous,
                                  textColor = AppColors.ghostWhite,
                                  command = fun () -> dispatch NewFillUpTapped
                              ) ]
                    )

                let items =
                    fillUps
                    |> List.map
                        (fun item ->
                            View.SwipeView(
                                backgroundColor = AppColors.silverSandLight,
                                gestureRecognizers =
                                    [ View.TapGestureRecognizer(
                                          command = (fun () -> dispatch (UpdateFillUp item)),
                                          numberOfTapsRequired = 2
                                      ) ],
                                rightItems =
                                    View.SwipeItems(
                                        items =
                                            [ View.SwipeItem(
                                                text = "Delete",
                                                backgroundColor = Color.Red,
                                                command = fun () -> dispatch (RemoveFillUp item)
                                              )
                                              View.SwipeItem(
                                                  text = "Edit",
                                                  backgroundColor = AppColors.silverSandMediumDark,
                                                  command = fun () -> dispatch (UpdateFillUp item)
                                              ) ]
                                    ),
                                content = FillUpCell.view item
                            ))

                View.CollectionView(items, emptyView = emptyView, selectionMode = SelectionMode.Single)

        View.ContentPage(
            View.AbsoluteLayout(
                [ content
                  View
                      .Button(text = "+",
                              fontSize = FontSize.fromValue 24.0,
                              backgroundColor = AppColors.mandarin,
                              textColor = AppColors.ghostWhite,
                              command = (fun () -> dispatch NewFillUpTapped),
                              padding = Thickness 10.0)
                      .WidthRequest(60.0)
                      .HeightRequest(60.0)
                      .ButtonCornerRadius(30)
                      .LayoutFlags(AbsoluteLayoutFlags.PositionProportional)
                      .LayoutBounds(Rectangle(0.90, 0.95, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize)) ]
            ),
            backgroundColor = AppColors.silverSandLight,
            title = "Fill Ups"
        )
