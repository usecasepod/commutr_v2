namespace CommutrV2.Views

open CommutrV2
open CommutrV2.Models.Vehicles
open CommutrV2.Models.FillUps
open CommutrV2.Views.FillUpListing
open CommutrV2.Components
open CommutrV2.Data.VehicleRepository
open CommutrV2.Utils.Loading
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module VehicleDetailsPage =
    type Model =
        { Vehicle: Vehicle
          FillUps: FillUp Loadable }

    type ExternalMsg =
        | NoOp
        | GoToUpdateFillUp of FillUpUpdate.CreateOrUpdate

    type Msg =
        | FillUpListingMsg of FillUpListing.Msg
        | FillUpUpdateMsg of FillUpUpdate.Msg
        | UpdateWhenFillUpSaved
        | NavigationPopped
        | AddFillUp

    let init vehicle =
        { Vehicle = vehicle; FillUps = Loading }, Cmd.ofMsg (FillUpListingMsg LoadFillUps)

    let handleFillUpListingExternalMsg externalMsg model =
        match externalMsg with
        | FillUpListing.ExternalMsg.NoOp -> Cmd.none
        | FillUpListing.ExternalMsg.NavigateToAdd -> Cmd.ofMsg AddFillUp

    let handleFillUpUpdateExternalMsg externalMsg =
        match externalMsg with
        | FillUpUpdate.ExternalMsg.GoBackAfterFillUpSaved -> Cmd.ofMsg UpdateWhenFillUpSaved
        | FillUpUpdate.ExternalMsg.NoOp -> Cmd.none

    let update msg (model: Model) =
        match msg with
        | FillUpListingMsg fillUpMsg ->
            let (listingModel, cmd, externalMsg) =
                FillUpListing.update
                    fillUpMsg
                    { Vehicle = model.Vehicle
                      FillUps = model.FillUps }

            let externalCmd =
                handleFillUpListingExternalMsg externalMsg model

            let batchCmd =
                Cmd.batch [ (Cmd.map FillUpListingMsg cmd)
                            externalCmd ]

            { model with
                  FillUps = listingModel.FillUps },
            batchCmd,
            ExternalMsg.NoOp
        | AddFillUp ->
            model, Cmd.none, ExternalMsg.GoToUpdateFillUp(FillUpUpdate.CreateOrUpdate.Create model.Vehicle.Id)

    let view model dispatch =
        let fillUpListingTab =
            FillUpListing.view
                { Vehicle = model.Vehicle
                  FillUps = model.FillUps }
                (FillUpListingMsg >> dispatch)

        View.TabbedPage(
            barBackgroundColor = AppColors.cinereousMediumDark,
            barTextColor = AppColors.silverSand,
            backgroundColor = AppColors.silverSand,
            children = [ fillUpListingTab ]
        )
