module AAG.Client.Model

open System

/// The Elmish application's model.
type HintLevel =
    | Info
    | Warning
    | Error

type Hint =
    { value: string
      level: HintLevel }
    static member Info = { value = ""; level = Info }
    static member Error value = { value = value; level = Error }
    static member Required = Hint.Error "Field is required"

type Model =
    { page: Endpoint.Page
      step: string option
      graph: AAG.Graph
      addVertexInput: string
      addAccessInput: string
      subjectInput: string
      subjectId: Guid option
      factorsInput: Guid Set }
    static member Init =
        { page = Endpoint.MainMenu
          step = None
          graph = AAG.Graph.Empty
          addVertexInput = ""
          addAccessInput = ""
          subjectInput = ""
          subjectId = None
          factorsInput = Set.empty }

    static member Example = { Model.Init with graph = AAG.Graph.Example }
