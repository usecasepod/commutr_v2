// Copyright 2018 Fabulous contributors. See LICENSE.md for license.
namespace CommutrV2.Android

open System

open Android.App
open Android.Content
open Android.Content.PM
open Android.Runtime
open Android.Views
open Android.Widget
open Android.OS
open Xamarin.Forms.Platform.Android

[<Activity(Label = "commutr_v2.Android",
           Icon = "@drawable/icon",
           Theme = "@style/MainTheme",
           MainLauncher = true,
           ConfigurationChanges =
               (ConfigChanges.ScreenSize
                ||| ConfigChanges.Orientation))>]
type MainActivity() =
    inherit FormsAppCompatActivity()

    override this.OnCreate(bundle: Bundle) =
        FormsAppCompatActivity.TabLayoutResource <- Resources.Layout.Tabbar
        FormsAppCompatActivity.ToolbarResource <- Resources.Layout.Toolbar
        base.OnCreate(bundle)

        Xamarin.Essentials.Platform.Init(this, bundle)

        Forms.SetFlags("SwipeView_Experimental")
        Xamarin.Forms.Forms.Init(this, bundle)

        let appcore = new CommutrV2.App()
        this.LoadApplication(appcore)

    override this.OnRequestPermissionsResult(requestCode: int, permissions: string [],
                                             [<GeneratedEnum>] grantResults: Android.Content.PM.Permission []) =
        Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults)

        base.OnRequestPermissionsResult(requestCode, permissions, grantResults)
