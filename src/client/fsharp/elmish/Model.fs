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
    | AnalysisCompromise

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
      subjectAccessColor: byte option
      subjectAccessName: string
      selectedFactors: Guid Set
      highlightedEdgeIds: Guid Set
      initiallyCompromisedVertexIds: Guid Set
      transitivelyCompromisedVertexIds: Guid Set
      json: string }
    static member Init =
        { page = Endpoint.Main
          step = MainMenu
          graph = AAG.Graph.Empty
          addAccessNameInput = ""
          modifyVertexNameInput = ""
          subjectVertexId = None
          subjectAccessColor = None
          subjectAccessName = ""
          selectedFactors = Set.empty
          highlightedEdgeIds = Set.empty
          initiallyCompromisedVertexIds = Set.empty
          transitivelyCompromisedVertexIds = Set.empty
          json = """{ "vertices": [] }""" }

    static member Example = { Model.Init with graph = AAG.Graph.Example }

let highlightAccess (access: AAG.Access) (model: Model) =
    let edges =
        AAG.getEdgesForAccess model.graph access
        |> List.map (fun e -> e.id)
        |> Set.ofList

    { model with highlightedEdgeIds = edges }

let deHighlightAccess (model: Model) =
    { model with highlightedEdgeIds = Set.empty }
