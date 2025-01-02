module AAG.Client.Msg

/// The Elmish application's update messages.
type Message =
    | SetPage of Endpoint.Page
    | AddVertex of string
    | UpdateVertexName of string
    | SelectVertexForNewAccess of string
    | SelectVertexForNewAccessById of string
    | SelectFactorsForNewAccessById of string
    | ContinueAddAccessSubject
    | CancelAddAccess
    | CancelAddVertex
    | ClearGraph
    | ExampleGraph
