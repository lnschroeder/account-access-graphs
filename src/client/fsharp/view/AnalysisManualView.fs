module AAG.Client.AnalysisManualView

open Model
open Bolero.Html

// Validity
// Open
let ``open`` model =
    { page = model.page
      view = AnalysisManual
      physics = model.physics
      edgeLabels = model.edgeLabels
      straightEdges = model.straightEdges
      graph = model.graph
      addAccessNameInput = model.addAccessNameInput
      modifyVertexNameInput = model.modifyVertexNameInput
      subjectVertexId = model.subjectVertexId
      subjectAccessColor = model.subjectAccessColor
      subjectAccessName = model.subjectAccessName
      selectedFactors = Set.empty
      highlightedEdgeIds = Set.empty
      initiallyCompromisedVertexIds = Set.empty
      transitivelyCompromisedVertexIds = Set.empty
      newComponentSelection = ""
      components = []
      componentNameInput = ""
      subjectComponentName = ""
      json = model.json }

// Functionality
// Handle actions
let handleClickedBackground model dispatch =
    if model.initiallyCompromisedVertexIds.IsEmpty then
        dispatch Msg.OpenMainMenuView
    else
        dispatch Msg.OpenAnalysisManual

let handleClickedVertex model (vertex: AAG.Vertex) dispatch =
    dispatch (
        Msg.UpdateCompromisedVertices(
            if Set.contains vertex.id model.initiallyCompromisedVertexIds then
                Set.remove vertex.id model.initiallyCompromisedVertexIds
            else
                Set.add vertex.id model.initiallyCompromisedVertexIds
        )
    )

// View
let private showFactor (model: Model) id =
    match AAG.tryFindVertexById id model.graph with
    | Some vertex ->
        Template
            .AnalysisManual
            .Factor()
            .Name(vertex.name)
            .Elt()
    | None ->
        Template
            .AnalysisManual
            .Factor()
            .Name("INVALID")
            .Elt()

let view (model: Model) dispatch =
    Template
        .AnalysisManual()
        .Factors(forEach model.initiallyCompromisedVertexIds (showFactor model))
        .FactorsHint((initiallyCompromisedVertexIds model).value)
        .BackButton(fun _ -> dispatch Msg.OpenMainMenuView)
        .Elt()
