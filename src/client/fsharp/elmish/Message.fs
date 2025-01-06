module AAG.Client.Msg

open System

/// The Elmish application's update messages.
type Message =
    | IgnoreAction
    | SetPage of Endpoint.Page
    | OpenMainMenuStep
    | OpenModifyVertexStep of Guid option
    | OpenModifyAccessStep of Guid option
    | ToggleFactorOfSubjectAccess of Guid * AAG.Vertex
    | ClickedDeleteAccess of Guid
    | ClickedClearGraph
    | ClickedExampleGraph
    | ModifiedVertexName of Guid * string
    | ModifiedAccessName of AAG.Access * string
