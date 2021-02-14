namespace CommutrV2.Views

open CommutrV2
open CommutrV2.Models.FillUps
open CommutrV2.Data.FillUpRepository
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open System
open CommutrV2.Models
open CommutrV2.Utils.Input

module FillUpUpdate =
    type Model = { FillUp: FillUp }

    type ExternalMsg =
        | NoOp
        | GoBackAfterFillUpSaved

    type Msg =
        | UpdateDate of DateTime
        | UpdateDistance of Distance.T
        | UpdateFuelAmount of Volume.T
        | UpdateNotes of string
        | UpdatePricePerFuelAmount of decimal
        | SaveFillUp
        | FillUpSaved


    type CreateOrUpdate =
        | Create of VehicleId.T
        | Update of FillUp

    let initModel vehicleId: Model =
        { FillUp =
              { Id = FillUpId.create 0
                Date = DateTime.Now
                Distance = Distance.zero
                FuelAmount = Volume.zero
                Notes = ""
                PricePerFuelAmount = 0m
                VehicleId = vehicleId } }

    let init createOrUpdate =
        let model =
            match createOrUpdate with
            | Create v -> initModel v
            | Update fillUp -> { FillUp = fillUp }

        model, Cmd.none

    let createOrUpdateFillUpAsync model =
        async {
            match model.FillUp.Id with
            | FillUpId.T.FillUpId 0 ->
                do! insertFillUp model.FillUp |> Async.Ignore
                return FillUpSaved
            | _ ->
                do! updateFillUp model.FillUp |> Async.Ignore
                return FillUpSaved
        }

    let update msg model =
        match msg with
        | UpdateDate date ->
            let m =
                { model with
                      FillUp = { model.FillUp with Date = date } }

            m, Cmd.none, ExternalMsg.NoOp
        | UpdateDistance distance ->
            let m =
                { model with
                      FillUp =
                          { model.FillUp with
                                Distance = distance } }

            m, Cmd.none, ExternalMsg.NoOp
        | UpdateFuelAmount fuel ->
            let m =
                { model with
                      FillUp = { model.FillUp with FuelAmount = fuel } }

            m, Cmd.none, ExternalMsg.NoOp
        | UpdateNotes notes ->
            let m =
                { model with
                      FillUp = { model.FillUp with Notes = notes } }

            m, Cmd.none, ExternalMsg.NoOp
        | UpdatePricePerFuelAmount price ->
            let m =
                { model with
                      FillUp =
                          { model.FillUp with
                                PricePerFuelAmount = price } }

            m, Cmd.none, ExternalMsg.NoOp
        | SaveFillUp ->
            let cmd =
                Cmd.ofAsyncMsg (createOrUpdateFillUpAsync model)

            model, cmd, ExternalMsg.NoOp
        | FillUpSaved -> model, Cmd.none, ExternalMsg.GoBackAfterFillUpSaved

    let view model dispatch =
        let titleText =
            match model.FillUp.Id with
            | FillUpId.T.FillUpId 0 -> "Create New Fill Up"
            | _ -> "Update Existing Fill Up"

        let totalCost =
            calculateTotal model.FillUp.FuelAmount model.FillUp.PricePerFuelAmount

        View.ContentPage(
            View.StackLayout(
                children =
                    [ View.Label(text = "Date", textColor = AppColors.cinereous, fontAttributes = FontAttributes.Bold)
                      View.DatePicker(
                          date = model.FillUp.Date,
                          dateSelected = fun e -> e.NewDate |> (UpdateDate >> dispatch)
                      )
                      View.Label(
                          text = "Miles", //TODO: Units!
                          textColor = AppColors.cinereous,
                          fontAttributes = FontAttributes.Bold
                      )
                      View.Entry(
                          placeholder = "360.0",
                          keyboard = Keyboard.Numeric,
                          text = $"%M{(Distance.value model.FillUp.Distance)}",
                          clearButtonVisibility = ClearButtonVisibility.WhileEditing,
                          textChanged =
                              fun e ->
                                  e.NewTextValue
                                  |> fun str ->
                                      match stringToDistance str with
                                      | Some d -> d
                                      | None -> model.FillUp.Distance
                                  |> (UpdateDistance >> dispatch)
                      )
                      View.Label(
                          text = "$/Gallon",
                          textColor = AppColors.cinereous,
                          fontAttributes = FontAttributes.Bold
                      )
                      View.Entry(
                          placeholder = "1.999",
                          keyboard = Keyboard.Numeric,
                          text = $"%M{model.FillUp.PricePerFuelAmount}",
                          clearButtonVisibility = ClearButtonVisibility.WhileEditing,
                          textChanged =
                              fun e ->
                                  e.NewTextValue
                                  |> fun str ->
                                      match stringToDecimal str with
                                      | Some d -> d
                                      | None -> model.FillUp.PricePerFuelAmount
                                  |> (UpdatePricePerFuelAmount >> dispatch)
                      )
                      View.Label(
                          text = "Gallons",
                          textColor = AppColors.cinereous,
                          fontAttributes = FontAttributes.Bold
                      )
                      View.Entry(
                          placeholder = "12.000",
                          keyboard = Keyboard.Numeric,
                          text = $"%M{(Volume.value model.FillUp.FuelAmount)}",
                          clearButtonVisibility = ClearButtonVisibility.WhileEditing,
                          textChanged =
                              fun e ->
                                  e.NewTextValue
                                  |> fun str ->
                                      match stringToVolume str with
                                      | Some v -> v
                                      | None -> model.FillUp.FuelAmount
                                  |> (UpdateFuelAmount >> dispatch)

                      )
                      View.Label(
                          text = "Total $",
                          textColor = AppColors.cinereous,
                          fontAttributes = FontAttributes.Bold
                      )
                      View.Entry(isEnabled = false, text = $"%M{totalCost}")
                      View.Button(
                          text = "Save Vehicle",
                          backgroundColor = AppColors.cinereous,
                          textColor = AppColors.ghostWhite,
                          command = fun () -> dispatch SaveFillUp
                      ) ],
                margin = Thickness 10.0
            ),
            backgroundColor = AppColors.silverSandLight,
            title = titleText
        )
