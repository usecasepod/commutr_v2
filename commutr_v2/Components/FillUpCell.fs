namespace CommutrV2.Components

open CommutrV2
open CommutrV2.Models.FillUps
open Fabulous.XamarinForms
open Xamarin.Forms

module FillUpCell =
    let view fillUp =
        View.StackLayout(
            padding = Thickness 5.0,
            children =
                [ View.Frame(
                      hasShadow = false,
                      backgroundColor = AppColors.ghostWhite,
                      padding = Thickness 5.0,
                      content =
                          View.FlexLayout(
                              margin = Thickness 5.0,
                              verticalOptions = LayoutOptions.Center,
                              horizontalOptions = LayoutOptions.Center,
                              alignItems = FlexAlignItems.Center,
                              justifyContent = FlexJustify.SpaceEvenly,
                              direction = FlexDirection.Row,
                              children =
                                  [ View.Label(text = fillUp.Date.ToShortDateString())
                                    View.Label(text = sprintf "$%M" fillUp.PricePerFuelAmount) ]
                          )
                  ) ]
        ) //TODO: units of measure
