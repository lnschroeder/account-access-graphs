module AAG.Client.Msg

open System

/// The Elmish application's update messages.
type Message =
    | IgnoreAction
    | SetPage of Endpoint.Page
    | OpenMainMenuStep
    | OpenModifyVertexStep of Guid option
    | OpenModifyAccessStep of AAG.Access option
    | OpenAnalysisCompromise
    | HighlightAccess of AAG.Access
    | DeHighlightAccess
    | ToggleFactorForSubject of AAG.Vertex
    | ToggleInitialCompromise of AAG.Vertex
    | ClickedDeleteVertex of AAG.Vertex
    | ClickedDeleteAccess of AAG.Access
    | ClickedClearGraph
    | ClickedExampleGraph
    | ModifiedVertexName of Guid * string
    | ModifiedAccessName of AAG.Access * string
    | ModifiedGraphJson of string
