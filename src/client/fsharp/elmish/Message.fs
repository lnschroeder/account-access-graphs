module AAG.Client.Msg

/// The Elmish application's update messages.
type Message =
    | SetPage of Endpoint.Page
    | AddVertex of string
    | UpdateVertexName of string
    | ClearGraph
    | Error of exn
    | ClearError
