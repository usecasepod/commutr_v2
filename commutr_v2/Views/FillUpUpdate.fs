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
open CommutrV2.Utils.Validation

module FillUpUpdate =
    type Model =
        { Id: FillUpId.T
          Date: DateTime
          FuelAmount: Validatable<Volume.T option>
          PricePerFuelAmount: Validatable<decimal option>
          Distance: Validatable<Distance.T option>
          Notes: string
          VehicleId: VehicleId.T }

    type ExternalMsg =
        | NoOp
        | GoBackAfterFillUpSaved

    type Msg =
        | UpdateDate of DateTime
        | UpdateDistance of Distance.T option
        | UpdatePricePerFuelAmount of decimal option
        | UpdateFuelAmount of Volume.T option
        | UpdateNotes of string
        | SaveFillUp
        | FillUpSaved

    type CreateOrUpdate =
        | Create of VehicleId.T
        | Update of FillUp

    let initModel vehicleId: Model =
        { Id = FillUpId.T.FillUpId 0
          Date = DateTime.Now
          Distance = None |> NeedsValidation
          FuelAmount = None |> NeedsValidation
          Notes = String.Empty
          PricePerFuelAmount = None |> NeedsValidation
          VehicleId = vehicleId }

    let fillUpToModel (fillUp: FillUp) =
        { Id = fillUp.Id
          Date = fillUp.Date
          Distance = Some fillUp.Distance |> Valid
          FuelAmount = Some fillUp.FuelAmount |> Valid
          Notes = fillUp.Notes
          PricePerFuelAmount = Some fillUp.PricePerFuelAmount |> Valid
          VehicleId = fillUp.VehicleId }

    let init createOrUpdate =
        let model =
            match createOrUpdate with
            | Create v -> initModel v
            | Update fillUp -> fillUpToModel fillUp

        model, Cmd.none

    let createOrUpdateFillUpAsync (fillUp: FillUp) =
        async {
            match fillUp.Id with
            | FillUpId.T.FillUpId 0 ->
                do! insertFillUp fillUp |> Async.Ignore
                return FillUpSaved
            | _ ->
                do! updateFillUp fillUp |> Async.Ignore
                return FillUpSaved
        }

    let update msg model =
        match msg with
        | UpdateDate date ->
            let m = { (model: Model) with Date = date }

            m, Cmd.none, ExternalMsg.NoOp
        | UpdateDistance distance ->
            let m =
                { model with
                      Distance = NeedsValidation distance }

            m, Cmd.none, ExternalMsg.NoOp
        | UpdatePricePerFuelAmount price ->
            let m =
                { model with
                      PricePerFuelAmount = NeedsValidation price }

            m, Cmd.none, ExternalMsg.NoOp
        | UpdateFuelAmount fuel ->
            let m =
                { model with
                      FuelAmount = NeedsValidation fuel }

            m, Cmd.none, ExternalMsg.NoOp
        | UpdateNotes notes ->
            let m = { model with Notes = notes }

            m, Cmd.none, ExternalMsg.NoOp
        | SaveFillUp ->
            let m =
                { model with
                      Distance = (shouldHaveValue model.Distance)
                      FuelAmount = shouldHaveValue model.FuelAmount
                      PricePerFuelAmount = shouldHaveValue model.PricePerFuelAmount }

            let cmd =
                match (m.Distance, m.FuelAmount, m.PricePerFuelAmount) with
                | (Valid distance, Valid fuelAmount, Valid price) ->
                    Cmd.ofAsyncMsg (
                        createOrUpdateFillUpAsync
                            { Id = m.Id
                              Date = m.Date
                              FuelAmount = fuelAmount.Value
                              PricePerFuelAmount = price.Value
                              Distance = distance.Value
                              Notes = m.Notes
                              VehicleId = m.VehicleId }
                    )
                | (_, _, _) -> Cmd.none


            model, cmd, ExternalMsg.NoOp
        | FillUpSaved -> model, Cmd.none, ExternalMsg.GoBackAfterFillUpSaved

    let view model dispatch =
        let titleText =
            match model.Id with
            | FillUpId.T.FillUpId 0 -> "Create New Fill Up"
            | _ -> "Update Existing Fill Up"

        let distance =
            match value model.Distance with
            | Some d -> sprintf "%M" (Distance.value d)
            | None -> String.Empty

        let pricePerFuelAmount =
            match value model.PricePerFuelAmount with
            | Some p -> sprintf "%M" p
            | None -> String.Empty

        let fuelAmount =
            match value model.FuelAmount with
            | Some fuel -> sprintf "%M" (Volume.value fuel)
            | None -> String.Empty

        let totalCost =
            calculateTotal (value model.FuelAmount) (value model.PricePerFuelAmount)

        View.ContentPage(
            View.StackLayout(
                children =
                    [ View.Label(text = "Date", textColor = AppColors.cinereous, fontAttributes = FontAttributes.Bold)
                      View.DatePicker(date = model.Date, dateSelected = fun e -> e.NewDate |> (UpdateDate >> dispatch))
                      View.Label(
                          text = "Miles", //TODO: Units!
                          textColor = AppColors.cinereous,
                          fontAttributes = FontAttributes.Bold
                      )
                      View.Entry(
                          placeholder = "360.0",
                          keyboard = Keyboard.Numeric,
                          text = distance,
                          clearButtonVisibility = ClearButtonVisibility.WhileEditing,
                          textChanged =
                              fun e ->
                                  e.NewTextValue
                                  |> stringToDistance
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
                          text = pricePerFuelAmount,
                          clearButtonVisibility = ClearButtonVisibility.WhileEditing,
                          textChanged =
                              fun e ->
                                  e.NewTextValue
                                  |> stringToDecimal
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
                          text = fuelAmount,
                          clearButtonVisibility = ClearButtonVisibility.WhileEditing,
                          textChanged =
                              fun e ->
                                  e.NewTextValue
                                  |> stringToVolume
                                  |> (UpdateFuelAmount >> dispatch)

                      )
                      View.Label(
                          text = "Total $",
                          textColor = AppColors.cinereous,
                          fontAttributes = FontAttributes.Bold
                      )
                      View.Entry(isEnabled = false, text = (sprintf "%M" totalCost))
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
