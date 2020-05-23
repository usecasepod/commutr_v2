namespace commutr_v2

open Fabulous.XamarinForms
open Fabulous.XamarinForms.SkiaSharp
open SkiaSharp.Views.Forms
open Xamarin.Forms

module VehicleItem =
    type Model =
        { Id: int
          Make: string
          Model: string
          Year: int
          IsPrimary: bool }

    type Msg = TogglePrimary of bool

    let init (initModel: Model) = initModel

    let update msg model =
        match msg with
        | TogglePrimary isPrimary -> { model with IsPrimary = isPrimary }

    let view (model: Model) dispatch =
        View.StackLayout
            (padding = Thickness 5.0,
             children =
                 [ View.Frame
                     (hasShadow = false,
                      backgroundColor = AppColors.ghostWhite,
                      padding = Thickness 5.0,
                      content =
                          View.FlexLayout
                              (margin = Thickness 5.0,
                               verticalOptions = LayoutOptions.Center,
                               horizontalOptions = LayoutOptions.Center,
                               alignItems = FlexAlignItems.Center,
                               justifyContent = FlexJustify.SpaceEvenly,
                               direction = FlexDirection.Row,
                               children =
                                   [ View.SKCanvasView
                                       (paintSurface =
                                           (fun args ->
                                               let info = args.Info
                                               let surface = args.Surface
                                               let canvas = surface.Canvas

                                               canvas.Clear()

                                               use paint =
                                                   new SkiaSharp.SKPaint(Style = SkiaSharp.SKPaintStyle.Fill,
                                                                         Color =
                                                                             (if model.IsPrimary then
                                                                                 AppColors.mandarin.ToSKColor()
                                                                              else
                                                                                  AppColors.silverSandLight.ToSKColor()),
                                                                         StrokeWidth = 5.0f)

                                               canvas.DrawCircle
                                                   (float32 (info.Width / 2), float32 (info.Height / 2), 10.0f, paint)),
                                        invalidate = true) //This will redraw on every call to update
                                     View.Label(text = model.Year.ToString())
                                     View.Label(text = model.Make)
                                     View.Label(text = model.Model) ])) ])
