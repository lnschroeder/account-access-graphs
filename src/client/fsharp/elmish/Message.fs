module AAG.Client.Msg

open System

/// The Elmish application's update messages.
type Message =
    | Error of exn
    | IgnoreAction
    | GotGraph of AAG.Graph
    | SetPage of Endpoint.Page
    | OpenMainMenuStep
    | OpenModifyVertexStep of Guid option
    | OpenModifyAccessStep of AAG.Access * string
    | OpenAnalysisCompromise
    | HighlightAccess of AAG.Access
    | DeHighlightAccess
    | ToggleFactorForSubject of AAG.Vertex
    | ToggleInitialCompromise of AAG.Vertex
    | ClickedDeleteVertex of Guid
    | ClickedDeleteAccess of AAG.Access option
    | ClickedClearGraph
    | ClickedExampleGraph
    | ClickedExample2Graph
    | ModifiedVertexName of Guid * string
    | ModifiedAccessName of AAG.Access option * string
    | ModifiedGraphJson of string
