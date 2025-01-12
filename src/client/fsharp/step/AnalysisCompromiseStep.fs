module AAG.Client.AnalysisCompromiseStep

open Model
open Bolero.Html

let private getFactorsHint (model: Model) =
    if model.initiallyCompromisedVertexIds.IsEmpty then
        Hint.Error "Select at least one vertex to be compromised"
    else
        Hint.Info

let ``open`` model =
    { page = model.page
      step = AnalysisCompromise
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

let handleClickedBackground dispatch = dispatch Msg.OpenMainMenuStep

let handleClickedVertex (vertex: AAG.Vertex) (model: Model) dispatch =
    dispatch (Msg.ToggleInitialCompromise vertex)

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

let private showFactor (model: Model) id =
    match AAG.findVertexById id model.graph with
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

let view jsRuntime (model: Model) dispatch =
    Template
        .AnalysisCompromise()
        .Factors(forEach model.initiallyCompromisedVertexIds (showFactor model))
        .FactorsHint((getFactorsHint model).value)
        .BackButton(fun _ -> dispatch Msg.OpenMainMenuStep)
        .Elt()
