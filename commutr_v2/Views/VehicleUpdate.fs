namespace CommutrV2.Views

open CommutrV2
open CommutrV2.Models.Vehicles
open CommutrV2.VehicleRepository
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module VehicleUpdate =
    type Model = { Vehicle: Vehicle }

    type ExternalMsg =
        | NoOp
        | GoBackAfterVehicleSaved

    type Msg =
        | UpdateMake of string
        | UpdateModel of string
        | UpdateYear of int
        | UpdateOdometer of decimal
        | UpdateNotes of string
        | UpdateIsPrimary of bool
        | SaveVehicle
        | VehicleSaved

    let initModel: Model =
        { Vehicle =
              { Id = 0
                Make = ""
                Model = ""
                Year = 0
                Odometer = 0m
                Notes = ""
                IsPrimary = false } }

    let init (vehicle: Vehicle option) =
        let model =
            match vehicle with
            | Some v -> { Vehicle = v }
            | None -> initModel

        model, Cmd.none

    let createOrUpdateVehicleAsync model =
        async {
            match model.Vehicle.Id with
            | 0 ->
                do! insertVehicle model.Vehicle |> Async.Ignore
                return VehicleSaved
            | _ ->
                do! updateVehicle model.Vehicle |> Async.Ignore
                return VehicleSaved
        }

    let update msg model =
        match msg with
        | UpdateMake make ->
            let m =
                { model with
                      Vehicle = { model.Vehicle with Make = make } }

            m, Cmd.none, ExternalMsg.NoOp
        | UpdateModel vModel ->
            let m =
                { model with
                      Vehicle = { model.Vehicle with Model = vModel } }

            m, Cmd.none, ExternalMsg.NoOp
        | UpdateYear year ->
            let m =
                { model with
                      Vehicle = { model.Vehicle with Year = year } }

            m, Cmd.none, ExternalMsg.NoOp
        | UpdateOdometer odometer ->
            let m =
                { model with
                      Vehicle =
                          { model.Vehicle with
                                Odometer = odometer } }

            m, Cmd.none, ExternalMsg.NoOp
        | UpdateNotes notes ->
            let m =
                { model with
                      Vehicle = { model.Vehicle with Notes = notes } }

            m, Cmd.none, ExternalMsg.NoOp
        | UpdateIsPrimary isPrimary ->
            let m =
                { model with
                      Vehicle =
                          { model.Vehicle with
                                IsPrimary = isPrimary } }

            m, Cmd.none, ExternalMsg.NoOp
        | SaveVehicle ->
            let cmd =
                Cmd.ofAsyncMsg (createOrUpdateVehicleAsync model)

            model, cmd, ExternalMsg.NoOp
        | VehicleSaved -> model, Cmd.none, ExternalMsg.GoBackAfterVehicleSaved

    let yearToString year =
        match year with
        | 0 -> ""
        | _ -> year.ToString()

    let stringToYear str =
        match str with
        | "" -> 0
        | _ -> str |> int

    let odometerToString o =
        match o with
        | 0m -> ""
        | _ -> o.ToString()

    let stringToOdometer str =
        match str with
        | "" -> 0m
        | _ -> str |> decimal

    let view model dispatch =
        let titleText =
            match model.Vehicle.Id with
            | 0 -> "New Vehicle"
            | _ -> sprintf "%i %s %s" model.Vehicle.Year model.Vehicle.Make model.Vehicle.Model

        View.ContentPage
            (View.StackLayout
                (children =
                    [ View.Label(text = "Make", textColor = AppColors.cinereous, fontAttributes = FontAttributes.Bold)
                      View.Entry
                          (placeholder = "Honda",
                           text = model.Vehicle.Make,
                           clearButtonVisibility = ClearButtonVisibility.WhileEditing,
                           textChanged = fun e -> e.NewTextValue |> (UpdateMake >> dispatch))
                      View.Label(text = "Model", textColor = AppColors.cinereous, fontAttributes = FontAttributes.Bold)
                      View.Entry
                          (placeholder = "Accord",
                           text = model.Vehicle.Model,
                           clearButtonVisibility = ClearButtonVisibility.WhileEditing,
                           textChanged = fun e -> e.NewTextValue |> (UpdateModel >> dispatch))
                      View.Label(text = "Year", textColor = AppColors.cinereous, fontAttributes = FontAttributes.Bold)
                      View.Entry
                          (placeholder = "2005",
                           text = yearToString model.Vehicle.Year,
                           keyboard = Keyboard.Numeric,
                           clearButtonVisibility = ClearButtonVisibility.WhileEditing,
                           textChanged =
                               fun e ->
                                   e.NewTextValue
                                   |> stringToYear
                                   |> (UpdateYear >> dispatch))
                      View.Label
                          (text = "Odometer", textColor = AppColors.cinereous, fontAttributes = FontAttributes.Bold)
                      View.Entry
                          (placeholder = "100000.00",
                           text = odometerToString model.Vehicle.Odometer,
                           keyboard = Keyboard.Numeric,
                           clearButtonVisibility = ClearButtonVisibility.WhileEditing,
                           textChanged =
                               fun e ->
                                   e.NewTextValue
                                   |> stringToOdometer
                                   |> (UpdateOdometer >> dispatch))
                      View.StackLayout
                          (children =
                              [ View.Label
                                  (text = "Is Primary?",
                                   textColor = AppColors.cinereous,
                                   fontAttributes = FontAttributes.Bold,
                                   verticalOptions = LayoutOptions.Center)
                                View.CheckBox
                                    (isChecked = model.Vehicle.IsPrimary,
                                     color = AppColors.mandarin,
                                     verticalOptions = LayoutOptions.Center,
                                     checkedChanged = fun e -> e.Value |> (UpdateIsPrimary >> dispatch)) ],
                           orientation = StackOrientation.Horizontal)
                      View.Button
                          (text = "Save Vehicle",
                           backgroundColor = AppColors.cinereous,
                           textColor = AppColors.ghostWhite,
                           command = fun () -> dispatch SaveVehicle) ],
                 margin = Thickness 10.0),
             backgroundColor = AppColors.silverSandLight,
             title = titleText)
