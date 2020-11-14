namespace CommutrV2.Views

open CommutrV2
open CommutrV2.Models
open Fabulous
open Fabulous.XamarinForms
open System

module VehicleUpdate =
    type Model = { Vehicle: Vehicle }

    type ExternalMsg =
        | NoOp
        | GoBackAfterVehicleAdded of Vehicle
        | GoBackAfterVehicleUpdated of Vehicle

    type Msg =
        | UpdateMake of string
        | UpdateModel of string
        | UpdateYear of int
        | UpdateIsPrimary of bool
        | SaveVehicle

    let initModel:Model =
        { Vehicle =
            { Id = Guid.Empty
              Make = ""
              Model = ""
              Year = 0
              IsPrimary = false } }

    let init (vehicle: Vehicle option) = 
        let model =
            match vehicle with
            | Some v -> { Vehicle = v}
            | None -> initModel

        model, Cmd.none

    let createOrUpdateVehicle model =
        //TODO: eventually this will be responsible for persisting
        match model.Vehicle.Id with
        | id when id = Guid.Empty ->
            let modelWithId = { model with Vehicle = { model.Vehicle with Id = Guid.NewGuid() }}
            modelWithId, Cmd.none, ExternalMsg.GoBackAfterVehicleAdded modelWithId.Vehicle
        | _ -> model, Cmd.none, ExternalMsg.GoBackAfterVehicleUpdated model.Vehicle

    let update msg model =
        match msg with
        | UpdateMake make ->
            let m = { model with Vehicle = { model.Vehicle with Make = make } }
            m, Cmd.none, ExternalMsg.NoOp
        | UpdateModel vModel ->
            let m = { model with Vehicle = { model.Vehicle with Model = vModel } }
            m, Cmd.none, ExternalMsg.NoOp
        | UpdateYear year ->
            let m = { model with Vehicle = { model.Vehicle with Year = year } }
            m, Cmd.none, ExternalMsg.NoOp
        | UpdateIsPrimary isPrimary ->
            let m = { model with Vehicle = { model.Vehicle with IsPrimary = isPrimary } }
            m, Cmd.none, ExternalMsg.NoOp
        | SaveVehicle -> createOrUpdateVehicle model //TODO: add to vehicle listing and navigate? (This is gonna be interesting)

    let view model dispatch =
        let label =
            match model.Vehicle.Id with
            | id when id = Guid.Empty -> View.Label(text = "Create a new vehicle")
            | _ -> View.Label(text = "Edit this vehicle")

        View.ContentPage
            (View.StackLayout
                (children =
                    [ label
                      View.Entry
                          (placeholder = "Make",
                           text = model.Vehicle.Make,
                           textChanged = fun e -> e.NewTextValue |> (UpdateMake >> dispatch))
                      View.Button
                          (text = "Add Vehicle",
                           backgroundColor = AppColors.cinereous,
                           textColor = AppColors.ghostWhite,
                           command = fun () -> dispatch SaveVehicle)]))
