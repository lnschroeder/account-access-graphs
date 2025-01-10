module AAG.Client.Model

open System

/// The Elmish application's model.
type HintLevel =
    | Info
    | Warning
    | Error

and Hint =
    { value: string
      level: HintLevel }
    static member Info = { value = ""; level = Info }
    static member Error value = { value = value; level = Error }
    static member Required = Hint.Error "Field is required"

and Step =
    | MainMenu
    | ModifyAccess
    | ModifyVertex

and DeserializedGraph =
    | Graph of AAG.Graph
    | ErrorMsg of Hint

and Model =
    { page: Endpoint.Page
      step: Step
      graph: AAG.Graph
      addAccessNameInput: string
      modifyVertexNameInput: string
      subjectVertexId: Guid option
      subjectAccessId: Guid option
      selectedFactors: Guid Set
      highlightedAccess: Guid option
      json: string }
    static member Init =
        { page = Endpoint.Main
          step = MainMenu
          graph = AAG.Graph.Empty
          addAccessNameInput = ""
          modifyVertexNameInput = ""
          subjectVertexId = None
          subjectAccessId = None
          selectedFactors = Set.empty
          highlightedAccess = None
          json = """{ "vertices": [] }""" }

    static member Example = { Model.Init with graph = AAG.Graph.Example }

let highlightAccess accessId (model: Model) =
    { model with highlightedAccess = accessId }

let deHighlightAccess (model: Model) =
    { model with highlightedAccess = None }
