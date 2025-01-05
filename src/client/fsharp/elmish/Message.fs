module AAG.Client.Msg

open System

/// The Elmish application's update messages.
type Message =
    | SetPage of Endpoint.Page
    | ClickedBackFromFactors
    | ClickedCancelAddAccess
    | ClickedCancelAddVertex
    | ClickedClearGraph
    | ClickedContinueSubject
    | ClickedExampleGraph
    | ClickedRemoveProvisionalFactor of Guid
    | ClickedSaveAddAccess of string option
    | ClickedSaveAddVertex // TODO remove Clicked Messages
    | ClickedVisNodeOnAddAccessPage of AAG.Vertex option
    | TypedAccessName of string
    | TypedSubjectName of string
    | TypedVertexName of string
