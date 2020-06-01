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

    let init initModel = initModel

    let update msg model =
        match msg with
        | UpdateMake make -> { model with Make = make }
        | UpdateModel m -> { model with Model = m }
        | UpdateYear year -> { model with Year = year }
        | UpdateIsPrimary isPrimary -> { model with IsPrimary = isPrimary }
        | SaveVehicle -> model //TODO: add to vehicle listing and navigate? (This is gonna be interesting)

    let view model =
        let label =
            match model.Id with
            | 0 -> View.Label(text = "Create a new vehicle")
            | _ -> View.Label(text = "Edit this vehicle")

        View.StackLayout(children = [ label ]) //TODO: display edit components
