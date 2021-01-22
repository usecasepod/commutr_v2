// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace CommutrV2

open CommutrV2.Views
open Fabulous
open Fabulous.XamarinForms
open CommutrV2.Models.Vehicles
open Xamarin.Forms

module App =
    type Msg =
        | VehicleListingMsg of VehicleListing.Msg
        | VehicleUpdateMsg of VehicleUpdate.Msg
        | GoToUpdateVehicle of Vehicle option
        | UpdateWhenVehicleSaved
        | GoToVehicleDetails of Vehicle
        | VehicleDetailsMsg of VehicleDetailsPage.Msg
        | NavigationPopped

    type Model =
        { VehicleListPageModel: VehicleListing.Model
          VehicleUpdatePageModel: VehicleUpdate.Model option
          VehicleDetailsPageModel: VehicleDetailsPage.Model option

          // Workaround Cmd limitation -- Can not pop a page in page stack and send Cmd at the same time
          // Otherwise it would pop pages 2 times in NavigationPage
          WorkaroundNavPageBug: bool
          WorkaroundNavPageBugPendingCmd: Cmd<Msg> }

    type Pages =
        { VehicleListing: ViewElement
          VehicleUpdate: ViewElement option
          VehicleDetails: ViewElement option }

    let init () =
        let listModel, listMsg = VehicleListing.init ()

        let initialModel =
            { VehicleListPageModel = listModel
              VehicleUpdatePageModel = None
              VehicleDetailsPageModel = None
              WorkaroundNavPageBug = false
              WorkaroundNavPageBugPendingCmd = Cmd.none }

        initialModel, (Cmd.map VehicleListingMsg listMsg)

    let handleVehicleListExternalMsg externalMsg =
        match externalMsg with
        | VehicleListing.ExternalMsg.NoOp -> Cmd.none
        | VehicleListing.ExternalMsg.NavigateToAdd -> Cmd.ofMsg (Msg.GoToUpdateVehicle None)
        | VehicleListing.ExternalMsg.NavigateToUpdate veh -> Cmd.ofMsg (Msg.GoToUpdateVehicle(Some veh))
        | VehicleListing.ExternalMsg.NavigateToDetails veh -> Cmd.ofMsg (Msg.GoToVehicleDetails veh)

    let handleVehicleUpdateExternalMsg externalMsg =
        match externalMsg with
        | VehicleUpdate.ExternalMsg.NoOp -> Cmd.none
        | VehicleUpdate.ExternalMsg.GoBackAfterVehicleSaved -> Cmd.ofMsg UpdateWhenVehicleSaved

    let navigationMapper (model: Model) =
        let editModel = model.VehicleUpdatePageModel
        let detailModel = model.VehicleDetailsPageModel

        match (editModel, detailModel) with
        | (Some _, None) ->
            { model with
                  VehicleUpdatePageModel = None }
        | (None, Some _) ->
            { model with
                  VehicleDetailsPageModel = None }
        | (_, _) -> model



    let update msg (model: Model) =
        match msg with
        | VehicleListingMsg msg ->
            let m, cmd, externalMsg =
                VehicleListing.update msg model.VehicleListPageModel

            let externalCmd = handleVehicleListExternalMsg externalMsg

            let batchCmd =
                Cmd.batch [ (Cmd.map VehicleListingMsg cmd)
                            externalCmd ]

            { model with VehicleListPageModel = m }, batchCmd
        | VehicleUpdateMsg msg ->
            let m, cmd, externalMsg =
                VehicleUpdate.update msg model.VehicleUpdatePageModel.Value

            let externalCmd =
                handleVehicleUpdateExternalMsg externalMsg

            let batchCmd =
                Cmd.batch [ (Cmd.map VehicleUpdateMsg cmd)
                            externalCmd ]

            { model with
                  VehicleUpdatePageModel = Some m },
            batchCmd
        | GoToUpdateVehicle vehicle ->
            let m, cmd = VehicleUpdate.init vehicle

            { model with
                  VehicleUpdatePageModel = Some m },
            (Cmd.map VehicleUpdateMsg cmd)

        | UpdateWhenVehicleSaved ->
            let listMsg =
                Cmd.ofMsg (VehicleListingMsg(VehicleListing.Msg.LoadVehicles))

            { model with
                  VehicleUpdatePageModel = None },
            listMsg
        | GoToVehicleDetails vehicle ->
            let m, cmd = VehicleDetailsPage.init vehicle

            { model with
                  VehicleDetailsPageModel = Some m },
            (Cmd.map VehicleDetailsMsg cmd)
        | VehicleDetailsMsg msg ->
            let m, cmd =
                VehicleDetailsPage.update msg model.VehicleDetailsPageModel.Value

            { model with
                  VehicleDetailsPageModel = Some m },
            Cmd.map VehicleDetailsMsg cmd
        | NavigationPopped ->
            match model.WorkaroundNavPageBug with
            | true ->
                //Do not pop pages if already done manually
                let newModel =
                    { model with
                          WorkaroundNavPageBug = false
                          WorkaroundNavPageBugPendingCmd = Cmd.none }

                newModel, model.WorkaroundNavPageBugPendingCmd
            | false -> navigationMapper model, Cmd.none

    let getPages allPages =
        let vehicleListing = allPages.VehicleListing
        let vehicleUpdate = allPages.VehicleUpdate
        let vehicleDetails = allPages.VehicleDetails

        match (vehicleUpdate, vehicleDetails) with
        | (Some update, None) -> [ vehicleListing; update ]
        | (None, Some detail) -> [ vehicleListing; detail ]
        | (_, _) -> [ vehicleListing ]

    let view model dispatch =
        let listingPage =
            VehicleListing.view model.VehicleListPageModel (VehicleListingMsg >> dispatch)

        let updatePage =
            model.VehicleUpdatePageModel
            |> Option.map (fun updateModel -> VehicleUpdate.view updateModel (VehicleUpdateMsg >> dispatch))

        let detailsPage =
            model.VehicleDetailsPageModel
            |> Option.map (fun detailModel -> VehicleDetailsPage.view detailModel (VehicleDetailsMsg >> dispatch))

        let allPages =
            { VehicleListing = listingPage
              VehicleUpdate = updatePage
              VehicleDetails = detailsPage }

        View.NavigationPage(
            barBackgroundColor = AppColors.cinereousMediumDark,
            barTextColor = AppColors.silverSand,
            backgroundColor = AppColors.silverSand,
            popped = (fun _ -> dispatch NavigationPopped),
            pages = getPages allPages
        )

    // Note, this declaration is needed if you enable LiveUpdate
    let program =
        XamarinFormsProgram.mkProgram init update view

type App() as app =
    inherit Application()

    let runner =
        App.program
#if DEBUG
        |> Program.withConsoleTrace
#endif
        |> XamarinFormsProgram.run app

    // #if DEBUG
    //     // Uncomment this line to enable live update in debug mode.
    //     // See https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/tools.html#live-update for further  instructions.
    //     //
    //     do runner.EnableLiveUpdate()
    // #endif

    // Uncomment this code to save the application state to app.Properties using Newtonsoft.Json
    // See https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/models.html#saving-application-state for further  instructions.
    let modelId = "model"

    override __.OnSleep() =

        let json =
            Newtonsoft.Json.JsonConvert.SerializeObject(runner.CurrentModel)

        app.Properties.[modelId] <- json

    override __.OnResume() =
        try
            match app.Properties.TryGetValue modelId with
            | true, (:? string as json) ->
                let model =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<App.Model>(json)

                runner.SetCurrentModel(model, Cmd.none)
            | _ -> ()
        with ex -> App.program.onError ("Error while restoring model found in app.Properties", ex)

    override this.OnStart() = this.OnResume()
