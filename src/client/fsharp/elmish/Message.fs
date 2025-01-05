module AAG.Client.Msg

open System

/// The Elmish application's update messages.
type Message =
    | SetPage of Endpoint.Page
    | ClickedAddAccess
    | ClickedAddVertex
    | ClickedDeleteAccess
    | ClickedClearGraph
    | ClickedExampleGraph
    | ClickedModifyAccess
    | ClickedRemoveProvisionalFactor of Guid
    | ClickedBackFromSubjectAccess
    | ClickedBackFromSubjectVertex
    | ClickedVisNode of string
    | ModifiedVertexName of Guid * string
    | TypedAccessName of string
