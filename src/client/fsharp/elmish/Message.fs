module AAG.Client.Msg

open System

/// The Elmish application's update messages.
type Message =
    | IgnoreAction
    | SetPage of Endpoint.Page
    | OpenMainMenuStep
    | OpenModifyVertexStep of Guid option
    | OpenModifyAccessStep of Guid option
    | ToggleFactorOfSubjectAccess of AAG.Vertex
    | ClickedDeleteAccess
    | ClickedClearGraph
    | ClickedExampleGraph
    | ClickedRemoveProvisionalFactor of Guid
    | ModifiedVertexName of Guid * string
    | ModifiedAccessName of Guid * string
