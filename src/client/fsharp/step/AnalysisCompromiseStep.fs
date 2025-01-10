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
      subjectAccessId = model.subjectAccessId
      selectedFactors = Set.empty
      highlightedAccess = None
      initiallyCompromisedVertexIds = Set.empty
      json = model.json }

let handleClickedBackground dispatch =
    dispatch Msg.OpenMainMenuStep

let handleClickedVertex (vertex: AAG.Vertex) (model: Model) dispatch =
    dispatch (Msg.ToggleInitialCompromise vertex)

let removeInitialCompromiseVertex vertexId (model: Model) =
    { model with
        // graph = AAG.removeFactorFromGraph vertexId model.graph
        initiallyCompromisedVertexIds = Set.remove vertexId model.initiallyCompromisedVertexIds }

let addInitialCompromiseVertex vertexId (model: Model) =
    { model with
        // graph = AAG.addFactorToGraph accessId vertexId model.graph
        initiallyCompromisedVertexIds = Set.add vertexId model.initiallyCompromisedVertexIds }

let toggleFactor (vertex: AAG.Vertex) (model: Model) =
    if  Set.contains vertex.id model.initiallyCompromisedVertexIds then
        removeInitialCompromiseVertex vertex.id model
    else
        addInitialCompromiseVertex vertex.id model


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
