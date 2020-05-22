namespace commutr_v2

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module VehicleItem = 
    type Model = 
      { Id : int
        Make : string
        Model : string
        Year : int
        IsPrimary : bool }

    type Msg = 
        | TogglePrimary of bool

    let init (initModel : Model) = initModel

    let update msg model = 
        match msg with 
        | TogglePrimary isPrimary -> { model with IsPrimary = isPrimary }

    let view (model: Model) dispatch = 
        View.StackLayout(padding = Thickness 20.0, verticalOptions = LayoutOptions.Center, orientation = StackOrientation.Horizontal,
            children = [ 
                View.Label(text = model.Make, horizontalOptions = LayoutOptions.Center, width=200.0, horizontalTextAlignment=TextAlignment.Center)
                View.Label(text = model.Model, horizontalOptions = LayoutOptions.Center, width=200.0, horizontalTextAlignment=TextAlignment.Center)
            ])