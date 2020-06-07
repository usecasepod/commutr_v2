namespace CommutrV2

open System.Net
open Newtonsoft.Json
open CommutrV2.Models
open CommutrV2.Views
open Fabulous
open Xamarin.Forms

[<QueryProperty("Vehicle", "vehicle")>]
type VehicleUpdatePage() =
    inherit ContentPage()

    let mutable _prevViewElement = None

    let mutable _vehicle = ""

    member this.Vehicle
        with get () = _vehicle
        and set (value) =
            _vehicle <- value
            this.Refresh()

    member this.Refresh() =
        let model =
            match _vehicle with
            | null
            | "" -> VehicleUpdate.init
            | vehStr -> { Vehicle = vehStr |> JsonConvert.DeserializeObject<Vehicle> }

        let viewElement =
            VehicleUpdate.view model (fun msg -> VehicleUpdate.update)

        match _prevViewElement with
        | None -> this.Content <- viewElement.Create() :?> View
        | Some prevViewElement -> viewElement.UpdateIncremental(prevViewElement, this.Content)
