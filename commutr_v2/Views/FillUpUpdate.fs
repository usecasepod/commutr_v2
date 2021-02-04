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
        | FuelAmount of decimal
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
                Distance = Distance.T.Distance 0.0m
                FuelAmount = 0m
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
        | FuelAmount fuel ->
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

        View.ContentPage(
            View.StackLayout(
                children =
                    [ View.Label(text = "Date", textColor = AppColors.cinereous, fontAttributes = FontAttributes.Bold)
                      View.DatePicker(
                          date = model.FillUp.Date,
                          dateSelected = fun e -> e.NewDate |> (UpdateDate >> dispatch)
                      )
                      View.Label(
                          text = "Distance (Miles)",
                          textColor = AppColors.cinereous,
                          fontAttributes = FontAttributes.Bold
                      )
                      View.Entry(
                          placeholder = "360.5",
                          keyboard = Keyboard.Numeric,
                          text = model.FillUp.Distance.ToString(),
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
