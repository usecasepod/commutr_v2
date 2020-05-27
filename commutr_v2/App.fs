// Copyright 2018-2019 Fabulous contributors. See LICENSE.md for license.
namespace Commutr_v2

open Commutr_v2.Views
open System.Diagnostics
open Fabulous
open Fabulous.XamarinForms
open Fabulous.XamarinForms.LiveUpdate
open Xamarin.Forms

module App =
    type Model = { Vehicles: VehicleListing.Model }

    type Msg =
        | VehicleListUpdated of VehicleListing.Msg
        | RefreshVehicles

    let shellRef = ViewRef<Shell>()

    let initModel = { Vehicles = VehicleListing.init () }

    let init () =
        Routing.RegisterRoute("vehicles", typeof<VehiclePage>)
        initModel, Cmd.none

    let update msg model =
        match msg with
        | VehicleListUpdated listMsg ->
            { model with
                  Vehicles = VehicleListing.update listMsg model.Vehicles },
            Cmd.none
        | RefreshVehicles ->
            { model with
                  Vehicles = VehicleListing.init () },
            Cmd.none

    let view (model: Model) (dispatch: Dispatch<Msg>) =
        let vehicleList =
            VehicleListing.view model.Vehicles (fun msg -> dispatch (VehicleListUpdated msg))

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
                               fontSize = FontSize.Named(NamedSize.Large),
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

#if DEBUG
    // Uncomment this line to enable live update in debug mode.
    // See https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/tools.html#live-update for further  instructions.
    //
    do runner.EnableLiveUpdate()
#endif

// Uncomment this code to save the application state to app.Properties using Newtonsoft.Json
// See https://fsprojects.github.io/Fabulous/Fabulous.XamarinForms/models.html#saving-application-state for further  instructions.
#if APPSAVE
    let modelId = "model"

    override __.OnSleep() =

        let json =
            Newtonsoft.Json.JsonConvert.SerializeObject(runner.CurrentModel)

        Console.WriteLine("OnSleep: saving model into app.Properties, json = {0}", json)

        app.Properties.[modelId] <- json

    override __.OnResume() =
        Console.WriteLine "OnResume: checking for model in app.Properties"
        try
            match app.Properties.TryGetValue modelId with
            | true, (:? string as json) ->

                Console.WriteLine("OnResume: restoring model from app.Properties, json = {0}", json)

                let model =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<App.Model>(json)

                Console.WriteLine("OnResume: restoring model from app.Properties, model = {0}", (sprintf "%0A" model))
                runner.SetCurrentModel(model, Cmd.none)

            | _ -> ()
        with ex -> App.program.onError ("Error while restoring model found in app.Properties", ex)

    override this.OnStart() =
        Console.WriteLine "OnStart: using same logic as OnResume()"
        this.OnResume()
#endif
