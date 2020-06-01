namespace CommutrV2

open System.Net
open Newtonsoft.Json
open CommutrV2.Models
open CommutrV2.Views
open Fabulous
open Xamarin.Forms

[<QueryProperty("Vehicle", "vehicle")>]
type VehicleUpdatePage(view: VehicleUpdate.Model -> ViewElement) =
    inherit ContentPage()

    let mutable _prevViewElement = None

    let mutable _vehicle =
        { Id = 0
          Make = ""
          Model = ""
          Year = 0
          IsPrimary = false }

    member this.Vehicle
        with get () = _vehicle
        and set (value: Vehicle) = _vehicle <- value

    member this.Refresh() =
        let viewElement = VehicleUpdate.view _vehicle
        match _prevViewElement with
        | None -> this.Content <- viewElement.Create() :?> View
        | Some prevViewElement -> viewElement.UpdateIncremental(prevViewElement, this.Content)
