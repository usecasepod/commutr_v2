namespace commutr_v2

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module VehicleListing = 
    type Model = { 
        Vehicles : VehicleItem.Model list
        SelectedVehicle : Option<VehicleItem.Model>
        }

    type Msg = 
        | Insert
        | Remove
        | Modified of int * VehicleItem.Msg
        | SelectVehicle of int

    let initModel : Model = { Vehicles = [
                            {Id = 1; Make = "Honda"; Model = "Accord"; Year = 2005; IsPrimary = false}
                            {Id = 2; Make = "Honda"; Model = "Insight"; Year = 2019; IsPrimary = true}
                            ];
                            SelectedVehicle = None }

    let init() = initModel

    let update msg model = 
        match msg with
        | Insert -> model //TODO: Make this do something
        | Remove -> model //TODO: Make this do something
        | Modified (pos, itemMessage) -> { model with Vehicles = model.Vehicles 
                                                        |> List.mapi (fun i itemModel -> 
                                                        if i = pos then 
                                                            VehicleItem.update itemMessage itemModel
                                                        else 
                                                            itemModel)}
        | SelectVehicle pos -> {model with SelectedVehicle = Some(model.Vehicles.Item(pos))}

    let view (model : Model) dispatch = 
        let items = model.Vehicles |> List.mapi(fun pos itemModel -> 
                             VehicleItem.view 
                                itemModel 
                                (fun msg -> dispatch (Modified (pos, msg))))
        View.CollectionView(items)