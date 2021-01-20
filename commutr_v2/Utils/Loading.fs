namespace CommutrV2.Utils

module Loading =
    type Loadable<'T> =
        | Loading
        | Loaded of 'T list
