namespace CommutrV2.Views

open CommutrV2
open CommutrV2.Models.Vehicles
open CommutrV2.Models.FillUps
open CommutrV2.Components
open CommutrV2.VehicleRepository
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module VehicleDetailsPage =
    type Model =
        { Vehicle: Vehicle
          FillUps: FillUp list }

    type Msg =
        | LoadFillUps
        | FillupsLoaded

    let init vehicle =
        { Vehicle = vehicle; FillUps = [] }, Cmd.ofMsg LoadFillUps

    let update msg model =
        match msg with
        | LoadFillUps -> 
