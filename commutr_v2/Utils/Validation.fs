namespace CommutrV2.Utils

module Validation =
    type Validatable<'T> =
        | NeedsValidation of 'T
        | Valid of 'T
        | HasError of 'T * string

    let value validatable =
        match validatable with
        | NeedsValidation value -> value
        | Valid value -> value
        | HasError (value, error) -> value

    let shouldHaveValue (validatable: Validatable<'T option>) =
        match value validatable with
        | Some v -> Some v |> Valid
        | None -> ((value validatable), "Required") |> HasError
