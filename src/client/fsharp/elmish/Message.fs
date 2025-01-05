module AAG.Client.Msg

open System

/// The Elmish application's update messages.
type Message =
    | SetPage of Endpoint.Page
    | ClickedAddAccess
    | ClickedAddVertex
    | ClickedBackFromFactors
    | ClickedCancelAddVertex
    | ClickedClearGraph
    | ClickedExampleGraph
    | ClickedModifyAccess
    | ClickedRemoveProvisionalFactor of Guid
    | ClickedSaveAddAccess of string option
    | ClickedSaveAddVertex // TODO remove Clicked Messages
    | ClickedVisNode of string
    | ModifiedVertexName of Guid * string
    | TypedAccessName of string
    | TypedVertexName of string
