namespace CommutrV2.Components

open CommutrV2
open CommutrV2.Models.Vehicles
open Fabulous.XamarinForms
open Fabulous.XamarinForms.SkiaSharp
open SkiaSharp.Views.Forms
open Xamarin.Forms

module VehicleCell =
    let init (initModel: Vehicle) = initModel

    let view (vehicle) =
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
                                                                             (if vehicle.IsPrimary then
                                                                                 AppColors.mandarin.ToSKColor()
                                                                              else
                                                                                  AppColors.silverSandLight.ToSKColor()),
                                                                         StrokeWidth = 5.0f)

                                               canvas.DrawCircle
                                                   (float32 (info.Width / 2), float32 (info.Height / 2), 10.0f, paint)),
                                        invalidate = true) //This will redraw on every call to update
                                     View.Label(text = vehicle.Year.ToString())
                                     View.Label(text = vehicle.Make)
                                     View.Label(text = vehicle.Model) ])) ])
