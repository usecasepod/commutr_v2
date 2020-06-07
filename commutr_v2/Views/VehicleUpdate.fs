namespace CommutrV2.Views

open CommutrV2
open CommutrV2.Models
open CommutrV2.Components
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module VehicleUpdate =
    type Model = { Vehicle: Vehicle }

    type Msg =
        | UpdateMake of string
        | UpdateModel of string
        | UpdateYear of int
        | UpdateIsPrimary of bool
        | SaveVehicle

    let init =
        { Vehicle =
              { Id = 0
                Make = ""
                Model = ""
                Year = 0
                IsPrimary = false } }

    let update msg model =
        match msg with
        | UpdateMake make ->
            { model with
                  Vehicle = { model.Vehicle with Make = make } }
        | UpdateModel m ->
            { model with
                  Vehicle = { model.Vehicle with Model = m } }
        | UpdateYear year ->
            { model with
                  Vehicle = { model.Vehicle with Year = year } }
        | UpdateIsPrimary isPrimary ->
            { model with
                  Vehicle =
                      { model.Vehicle with
                            IsPrimary = isPrimary } }
        | SaveVehicle -> model //TODO: add to vehicle listing and navigate? (This is gonna be interesting)

    let view model dispatch =
        let label =
            match model.Vehicle.Id with
            | 0 -> View.Label(text = "Create a new vehicle")
            | _ -> View.Label(text = "Edit this vehicle")

        View.StackLayout
            (children =
                [ label
                  View.Entry
                      (placeholder = "Make",
                       text = model.Vehicle.Make,
                       textChanged = fun e -> e.NewTextValue |> (UpdateMake >> dispatch)) ]) //TODO: display edit components
