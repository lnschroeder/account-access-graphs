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
    | AddVertex
    | AddAccessFactors
    | ModifyVertex

and Model =
    { page: Endpoint.Page
      step: Step
      graph: AAG.Graph
      addVertexInput: string
      addAccessNameInput: string
      modifyVertexNameInput: string
      subjectId: Guid option
      factorsInput: Guid list }
    static member Init =
        { page = Endpoint.Main
          step = MainMenu
          graph = AAG.Graph.Empty
          addVertexInput = ""
          addAccessNameInput = ""
          modifyVertexNameInput = ""
          subjectId = None
          factorsInput = [] }

    static member Example = { Model.Init with graph = AAG.Graph.Example }
