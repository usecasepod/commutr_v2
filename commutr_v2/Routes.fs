namespace CommutrV2

open CommutrV2.Views
open Fabulous
open Xamarin.Forms

type VehiclePage(view: VehicleListing.Model -> ViewElement) =
    inherit ContentPage()

    let mutable _prevViewElement = None

    member this.Refresh() =
        let viewElement = view VehicleListing.initModel
        match _prevViewElement with
        | None -> this.Content <- viewElement.Create() :?> View
        | Some prevViewElement -> viewElement.UpdateIncremental(prevViewElement, this.Content)
