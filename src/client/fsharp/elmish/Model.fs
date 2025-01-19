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
    | AnalysisScore
    | Vinit

and DeserializedGraph =
    | Graph of AAG.Graph
    | ErrorMsg of Hint

and Model =
    { page: Endpoint.Page
      step: Step
      graph: AAG.Graph
      physics: bool
      edgeLabels: bool
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
          physics = true
          edgeLabels = false
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

let accessNameInputPlaceholder (model: Model) =
    $"Defaults to: {model.subjectAccessName}"

let isValidTextValue (value: string) = value <> "" && value = value.Trim()

let selectedFactors (model: Model) =
    match model.subjectVertexId with
    | Some subjectVertexId ->
        let value = model.selectedFactors
        if value.IsEmpty then
            Hint.Error "Select at least one factor or delete the access"
        elif Seq.contains subjectVertexId value then
            Hint.Error "Self-references are not allowed"
        elif Seq.length (AAG.getAccessesWithFactors subjectVertexId value model.graph) > 1 then
            Hint.Error "There is already an access with the same factors"
        else
            Hint.Info
    | None -> Hint.Error "No subject vertex selected yet"

let addAccessNameInput (model: Model) =
    match model.subjectVertexId with
    | Some subjectVertexId ->
        let value = model.addAccessNameInput

        if value = "" then
            Hint.Info
        elif not (isValidTextValue value) then
            Hint.Error "No leading and trailing whitespaces allowed"
        elif Seq.length (AAG.getAccessesWithName subjectVertexId value model.graph) > 1 then
            Hint.Error "Access name already taken for that vertex"
        else
            Hint.Info
    | None -> Hint.Error "Select a subject vertex first!"

let modifyVertexNameInput (model: Model) =
    let value = model.modifyVertexNameInput

    if value = "" then
        Hint.Required
    elif not (isValidTextValue value) then
        Hint.Error "No leading and trailing whitespaces allowed"
    elif Seq.length (AAG.getVerticesWithName value model.graph) > 1 then
        Hint.Error "Vertex already exists"
    else
        Hint.Info

let initiallyCompromisedVertexIds (model: Model) =
    if model.initiallyCompromisedVertexIds.IsEmpty then
        Hint.Error "Select at least one vertex to be compromised"
    else
        Hint.Info

let isValid (model: Model) f =
    (f model).level <> Error

let highlightAccess (access: AAG.Access) (model: Model) =
    let edges =
        AAG.getEdgesForAccess model.graph access
        |> List.map (fun e -> e.id)
        |> Set.ofList

    { model with highlightedEdgeIds = edges }

let deHighlightAccess (model: Model) =
    { model with highlightedEdgeIds = Set.empty }
