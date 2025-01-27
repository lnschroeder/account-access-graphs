module AAG.Client.AnalysisAutomatedStep

open Model
open Bolero.Html

// Validity
// Handle actions
let handleClickedVertex (vertex: AAG.Vertex) dispatch =
    dispatch (Msg.OpenAnalysisForVertex(Some vertex.id))

let handleClickedBackground dispatch =
    dispatch (Msg.OpenAnalysisForVertex None)


// Functionality
let private recalculateGraph graph = AAG.computeAccessBase graph

// Open
let private openPrivate model graph vertexId =
    { page = model.page
      step = AnalysisAutomated
      physics = model.physics
      edgeLabels = model.edgeLabels
      graph = graph
      addAccessNameInput = model.addAccessNameInput
      modifyVertexNameInput = model.modifyVertexNameInput
      subjectVertexId = vertexId
      subjectAccessColor = model.subjectAccessColor
      subjectAccessName = model.subjectAccessName
      selectedFactors = Set.empty
      highlightedEdgeIds = Set.empty
      initiallyCompromisedVertexIds = Set.empty
      transitivelyCompromisedVertexIds = Set.empty
      json = model.json }

let ``open`` model =
    openPrivate model (recalculateGraph model.graph) None

let showAnalysisForSubject model vertexId = openPrivate model model.graph vertexId

// View
let private showAccessSetFactor (graph: AAG.Graph) factorId =
    Template
        .AnalysisAutomated
        .AccessSetFactor()
        .AccessSetFactorName(
            AAG.tryFindVertexById factorId graph
            |> Option.map (fun v -> v.name)
            |> Option.defaultValue "Error"
        )
        .Elt()

let private showAccessSet dispatch (graph: AAG.Graph) (accessSet: AAG.AccessSet) =
    Template
        .AnalysisAutomated
        .AccessSet()
        .Score((AAG.getScore graph accessSet).ToString())
        .AccessSetFactors(forEach accessSet.factors (showAccessSetFactor graph))
        .Enter(fun _ -> dispatch (Msg.UpdateCompromisedVertices accessSet.factors))
        .Leave(fun _ -> dispatch (Msg.UpdateCompromisedVertices Set.empty))
        .Elt()

let view (model: Model) dispatch =
    let template =
        match model.subjectVertexId with
        | Some subjectVertexId ->
            match AAG.tryFindVertexById subjectVertexId model.graph with
            | Some subjectVertex ->
                Template
                    .AnalysisAutomated()
                    .AccessBase(forEach subjectVertex.accessBase.accessSets (showAccessSet dispatch model.graph))
            | None -> Template.AnalysisAutomated()
        | None -> Template.AnalysisAutomated()

    template
        .BackButton(fun _ -> dispatch Msg.OpenMainMenuStep)
        .Elt()
