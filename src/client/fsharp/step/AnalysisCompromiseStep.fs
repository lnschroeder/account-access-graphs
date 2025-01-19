module AAG.Client.AnalysisCompromiseStep

open Model
open Bolero.Html

// Validity
// Handle actions
let handleClickedBackground model dispatch =
    if model.initiallyCompromisedVertexIds.IsEmpty then
        dispatch Msg.OpenMainMenuStep
    else
        dispatch Msg.OpenAnalysisCompromise


let handleClickedVertex (vertex: AAG.Vertex) dispatch =
    dispatch (Msg.ToggleInitialCompromise vertex)

// Open
let ``open`` model =
    { page = model.page
      step = AnalysisCompromise
      physics = model.physics
      edgeLabels = model.edgeLabels
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
      json = model.json }

// Functionality
let private removeInitialCompromiseVertex (vertex: AAG.Vertex) (model: Model) =
    let initiallyCompromisedVertexIds =
        Set.remove vertex.id model.initiallyCompromisedVertexIds

    { model with
        initiallyCompromisedVertexIds = initiallyCompromisedVertexIds
        transitivelyCompromisedVertexIds = AAG.getCompromisedVerticesOfGraph initiallyCompromisedVertexIds model.graph }

let private addInitialCompromiseVertex (vertex: AAG.Vertex) (model: Model) =
    let initiallyCompromisedVertexIds =
        Set.add vertex.id model.initiallyCompromisedVertexIds

    { model with
        initiallyCompromisedVertexIds = initiallyCompromisedVertexIds
        transitivelyCompromisedVertexIds = AAG.getCompromisedVerticesOfGraph initiallyCompromisedVertexIds model.graph }

let toggleFactor (vertex: AAG.Vertex) (model: Model) =
    if Set.contains vertex.id model.initiallyCompromisedVertexIds then
        removeInitialCompromiseVertex vertex model
    else
        addInitialCompromiseVertex vertex model

// View
let private showFactor (model: Model) id =
    match AAG.tryFindVertexById id model.graph with
    | Some vertex ->
        Template
            .AnalysisCompromise
            .Factor()
            .Name(vertex.name)
            .Elt()
    | None ->
        Template
            .AnalysisCompromise
            .Factor()
            .Name("INVALID")
            .Elt()

let view (model: Model) dispatch =
    Template
        .AnalysisCompromise()
        .Factors(forEach model.initiallyCompromisedVertexIds (showFactor model))
        .FactorsHint((initiallyCompromisedVertexIds model).value)
        .BackButton(fun _ -> dispatch Msg.OpenMainMenuStep)
        .Elt()
