module AAG.Client.Msg

/// The Elmish application's update messages.
type Message =
    | SetPage of Endpoint.Page
    | AddNode of string
    | UpdateNodeName of string
    | Error of exn
    | ClearError
