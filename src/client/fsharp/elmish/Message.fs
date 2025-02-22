module AAG.Client.Msg

open System

/// The Elmish application's update messages.
type Message =
    | Error of exn
    | IgnoreAction
    | GotGraph of AAG.Graph
    | GotComponents of string list
    | GotComponent of AAG.Component
    | SetComponentSelection of string
    | ImportSelectedComponent
    | ModifiedComponentName of string
    | ToggleOptionalAccessMethod of bool * AAG.OptionalAccessMethod
    | ToggleCondition of bool * AAG.Condition
    | SetPage of Endpoint.Page
    | SetPhysics of bool
    | SetEdgeLabels of bool
    | SetStraightEdges of bool
    | OpenMainMenuView
    | OpenModifyVertexView of Guid option
    | OpenModifyAccessView of AAG.Access * string
    | OpenAnalysisManual
    | OpenAnalysis
    | OpenAnalysisForVertex of Guid option
    | OpenVinit
    | OpenAddComponentView
    | OpenModifyComponent of string
    | UnsetVinit of AAG.Vertex
    | UpdateCompromisedVertices of Guid Set
    | SetVinit of AAG.Vertex
    | SetScore of AAG.Vertex * int
    | HighlightAccess of AAG.Access
    | DeHighlightAccess
    | ToggleFactorForSubject of AAG.Vertex
    | ClickedDeleteVertex of Guid
    | ClickedDeleteComponent of string
    | ClickedDeleteAccess of AAG.Access option
    | ClickedClearGraph
    | ClickedExampleGraph
    | ModifiedVertexName of Guid * string
    | ModifiedAccessName of AAG.Access option * string
    | ModifiedGraphJson of string
