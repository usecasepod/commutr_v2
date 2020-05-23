namespace commutr_v2

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module VehicleListing =
    type Model =
        { Vehicles: VehicleItem.Model list
          SelectedVehicle: Option<VehicleItem.Model> }

    type Msg =
        | Insert
        | Remove of int
        | Modified of int * VehicleItem.Msg
        | SelectVehicle of int

    let initModel: Model =
        { Vehicles =
              [ { Id = 1
                  Make = "Honda"
                  Model = "Accord"
                  Year = 2005
                  IsPrimary = false }
                { Id = 2
                  Make = "Honda"
                  Model = "Insight"
                  Year = 2019
                  IsPrimary = true } ]
          SelectedVehicle = None }

    let init () = initModel

    let update msg model =
        match msg with
        | Insert -> model //TODO: Make this do something
        | Remove id ->
            { model with
                  Vehicles =
                      model.Vehicles
                      |> List.filter (fun item -> item.Id <> id) }
        | Modified (pos, itemMessage) ->
            match itemMessage with
            | VehicleItem.Msg.TogglePrimary isPrimary ->
                { model with
                      Vehicles =
                          model.Vehicles
                          |> List.mapi (fun i itemModel ->
                              if i = pos then
                                  VehicleItem.update itemMessage itemModel
                              else
                                  (if isPrimary && itemModel.IsPrimary
                                   then VehicleItem.update (VehicleItem.TogglePrimary(false)) itemModel
                                   else itemModel)) }
        | SelectVehicle pos ->
            { model with
                  SelectedVehicle = Some(model.Vehicles.Item(pos)) }

    let view (model: Model) dispatch =
        let items =
            model.Vehicles
            |> List.mapi (fun pos itemModel ->
                View.SwipeView
                    (backgroundColor = AppColors.silverSandLight,
                     rightItems =
                         View.SwipeItems
                             (items =
                                 [ View.SwipeItem
                                     (text = "Delete",
                                      backgroundColor = Color.Red,
                                      command = fun () -> dispatch (Remove itemModel.Id)) ]),
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
                                                  (Modified(pos, VehicleItem.TogglePrimary(not itemModel.IsPrimary)))) ]),
                     content = VehicleItem.view itemModel (fun msg -> dispatch (Modified(pos, msg)))))

        View.CollectionView
            (items,
             emptyView =
                 View.StackLayout(spacing = 5.0, children = [ View.Label(text = "You don't have any vehicles!") ]))
