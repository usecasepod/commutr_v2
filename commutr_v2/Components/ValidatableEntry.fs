namespace CommutrV2.Components

open CommutrV2.Utils.Validation
open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms

module ValidatableEntry =
    type Model<'T> =
        { EntryView: string -> Entry
          Validatable: Validatable<'T> }
    let view(model) =
        ignore //TODO: actually do validation
    