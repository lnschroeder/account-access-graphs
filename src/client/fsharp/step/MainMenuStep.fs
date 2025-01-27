module AAG.Client.MainMenuStep

open Model

let clearGraph = Model.Init

let exampleGraph = Model.Example
// Validity
// Handle actions
let handleClickedVertex (vertex: AAG.Vertex) dispatch =
    dispatch (Msg.OpenModifyVertexStep(Some vertex.id))

let handleClickedAccess (access: AAG.Access) dispatch =
    dispatch (Msg.OpenModifyAccessStep(access, access.name))

let handleClickedBackground dispatch =
    dispatch (Msg.OpenModifyVertexStep None)
// Open
let ``open`` model =
    { page = Endpoint.Main
      step = MainMenu
      physics = model.physics
      edgeLabels = model.edgeLabels
      graph = model.graph
      addAccessNameInput = ""
      modifyVertexNameInput = ""
      subjectVertexId = None
      subjectAccessColor = None
      subjectAccessName = model.subjectAccessName
      selectedFactors = Set.empty
      highlightedEdgeIds = Set.empty
      initiallyCompromisedVertexIds = Set.empty
      transitivelyCompromisedVertexIds = Set.empty
      json = model.json }

// Functionality
// View

let view dispatch =
    Template
        .MainMenu()
        .AddVertexButton(fun _ -> dispatch (Msg.OpenModifyVertexStep None))
        .AnalysisCompromiseButton(fun _ -> dispatch (Msg.OpenAnalysisManual))
        .AnalysisScore(fun _ -> dispatch Msg.OpenAnalysis)
        .VinitButton(fun _ -> dispatch (Msg.OpenVinit))
        .ClearGraphButton(fun _ -> dispatch (Msg.ClickedClearGraph))
        .ExampleGraphButton(fun _ -> dispatch (Msg.ClickedExampleGraph))
        .ExampleGraph2Button(fun _ -> dispatch (Msg.ClickedExample2Graph))
        .Elt()
