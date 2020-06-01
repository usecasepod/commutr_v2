// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace CommutrV2

open CommutrV2.Views
open System.Diagnostics
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms

module App =
    type Model = { Vehicles: VehicleListing.Model }

    type Msg = VehicleListUpdated of VehicleListing.Msg

    let shellRef = ViewRef<Shell>()

    let initModel = { Vehicles = VehicleListing.init () }

    let init () =
        Routing.RegisterRoute("vehicle", typeof<VehicleUpdatePage>)
        initModel, Cmd.none

    let navigate route queryId param =
        match shellRef.TryValue with
        | None -> ()
        | Some shell ->
            let route =
                ShellNavigationState.op_Implicit (sprintf "%s?%s=%s" route queryId param)

            async {
                // Selecting an item in SearchHandler and immediately asking for navigation doesn't work on iOS.
                // This is a bug in Xamarin.Forms (https://github.com/xamarin/Xamarin.Forms/issues/5713)
                // The workaround is to wait for the fade out animation of SearchHandler to finish
                if Device.RuntimePlatform = Device.iOS then do! Async.Sleep 1000

                shell.FlyoutIsPresented <- false
                do! shell.GoToAsync route |> Async.AwaitTask
            }
            |> Async.StartImmediate

    let update msg model =
        match msg with
        | VehicleListUpdated listMsg ->
            { model with
                  Vehicles = VehicleListing.update listMsg model.Vehicles },
            Cmd.none

    let view model dispatch =
        let vehicleList =
            VehicleListing.view model.Vehicles (VehicleListUpdated >> dispatch)

        View.Shell
            (ref = shellRef,
             flyoutBackgroundColor = AppColors.silverSand,
             shellForegroundColor = AppColors.cinereousMediumDark,
             shellBackgroundColor = AppColors.silverSand,
             flyoutHeader =
                 View.StackLayout
                     (margin = Thickness(25.0, 75.0, 25.0, 25.0),
                      padding = Thickness 50.0,
                      children =
                          [ View.Label
                              (text = "Welcome to Commutr!",
                               textColor = AppColors.cinereousMediumDark,
                               fontSize = Named(NamedSize.Large),
                               horizontalTextAlignment = TextAlignment.Center) ]),
             items =
                 [ View.FlyoutItem
                     (title = "Vehicles",
                      route = "vehicles",
                      items =
                          [ View.ShellContent
                              (content =
                                  View.ContentPage(backgroundColor = AppColors.silverSandLight, content = vehicleList)) ]) ])

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
