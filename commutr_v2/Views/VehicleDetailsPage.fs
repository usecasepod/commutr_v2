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
    type Pages =
        { DetailsTabbedPage: ViewElement
          FillUpUpdatePage: ViewElement option }

    type Model =
        { Vehicle: Vehicle
          FillUps: FillUp Loadable
          FillUpUpdatePageModel: FillUpUpdate.Model option }

    type Msg =
        | FillUpListingMsg of FillUpListing.Msg
        | FillUpUpdateMsg of FillUpUpdate.Msg
        | UpdateWhenFillUpSaved
        | NavigationPopped

    let init vehicle =
        { Vehicle = vehicle
          FillUps = Loading
          FillUpUpdatePageModel = None },
        Cmd.ofMsg (FillUpListingMsg LoadFillUps)

    let handleFillUpListingExternalMsg externalMsg =
        match externalMsg with
        | FillUpListing.ExternalMsg.NoOp -> Cmd.none

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
                handleFillUpListingExternalMsg externalMsg

            let batchCmd =
                Cmd.batch [ (Cmd.map FillUpListingMsg cmd)
                            externalCmd ]

            { model with
                  FillUps = listingModel.FillUps },
            batchCmd
        | FillUpUpdateMsg fillUpUpdateMsg ->
            let (updateModel, cmd, externalMsg) =
                FillUpUpdate.update fillUpUpdateMsg model.FillUpUpdatePageModel.Value

            let externalCmd =
                handleFillUpUpdateExternalMsg externalMsg

            let batchCmd =
                Cmd.batch [ (Cmd.map FillUpUpdateMsg cmd)
                            externalCmd ]

            { model with
                  FillUpUpdatePageModel = Some updateModel },
            batchCmd

    let getPages allPages =
        let tabbedPage = allPages.DetailsTabbedPage

        match allPages.FillUpUpdatePage with
        | None -> [ tabbedPage ]
        | Some fillUpUpdatePage -> [ fillUpUpdatePage; tabbedPage ]

    let view model dispatch =
        let fillUpListingTab =
            FillUpListing.view
                { Vehicle = model.Vehicle
                  FillUps = model.FillUps }
                (FillUpListingMsg >> dispatch)

        let allPages =
            { DetailsTabbedPage = View.TabbedPage(children = [ fillUpListingTab ])
              FillUpUpdatePage =
                  model.FillUpUpdatePageModel
                  |> Option.map (fun model -> FillUpUpdate.view model (FillUpUpdateMsg >> dispatch)) }

        View.NavigationPage(
            barBackgroundColor = AppColors.cinereousMediumDark,
            barTextColor = AppColors.silverSand,
            backgroundColor = AppColors.silverSand,
            popped = (fun _ -> dispatch NavigationPopped),
            pages = getPages allPages
        )
