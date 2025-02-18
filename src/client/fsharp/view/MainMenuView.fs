module AAG.Client.MainMenuView

open Model

let clearGraph = Model.Init

// Validity
// Handle actions
let handleClickedVertex (vertex: AAG.Vertex) dispatch =
    dispatch (Msg.OpenModifyVertexView(Some vertex.id))

let handleClickedAccess (access: AAG.Access) dispatch =
    dispatch (Msg.OpenModifyAccessView(access, access.name))

let handleClickedBackground dispatch =
    dispatch (Msg.OpenModifyVertexView None)
// Open
let ``open`` model =
    { page = Endpoint.Main
      view = MainMenu
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
      newComponentSelection = ""
      components = []
      componentNameInput = ""
      subjectComponentName = ""
      json = model.json }

// Functionality
// View

let view dispatch =
    Template
        .MainMenu()
        .AddVertexButton(fun _ -> dispatch (Msg.OpenModifyVertexView None))
        .AddComponentButton(fun _ -> dispatch Msg.OpenAddComponentView)
        .AnalysisCompromiseButton(fun _ -> dispatch (Msg.OpenAnalysisManual))
        .AnalysisScore(fun _ -> dispatch Msg.OpenAnalysis)
        .VinitButton(fun _ -> dispatch (Msg.OpenVinit))
        .ClearGraphButton(fun _ -> dispatch (Msg.ClickedClearGraph))
        .ExampleGraphButton(fun _ -> dispatch (Msg.ClickedExampleGraph))
        .Elt()
