module AAG.Client.VinitView

open Model
open Bolero.Html

// Validity
// Handle actions
let handleClickedBackground dispatch = dispatch Msg.OpenMainMenuView

let handleClickedVertex (vertex: AAG.Vertex) dispatch =
    if vertex.isVinit then
        dispatch (Msg.UnsetVinit vertex)
    else
        dispatch (Msg.SetVinit vertex)

// Open
let ``open`` model =
    { page = model.page
      view = Vinit
      physics = model.physics
      edgeLabels = model.edgeLabels
      straightEdges = model.straightEdges
      graph = model.graph
      addAccessNameInput = model.addAccessNameInput
      modifyVertexNameInput = model.modifyVertexNameInput
      subjectVertexId = model.subjectVertexId
      subjectAccessColor = model.subjectAccessColor
      subjectAccessName = model.subjectAccessName
      selectedFactors = model.selectedFactors
      highlightedEdgeIds = model.highlightedEdgeIds
      initiallyCompromisedVertexIds = model.initiallyCompromisedVertexIds
      transitivelyCompromisedVertexIds = model.transitivelyCompromisedVertexIds
      newComponentSelection = ""
      components = []
      componentNameInput = ""
      subjectComponentName = ""
      json = model.json }

// Functionality

let setVinit (vertex: AAG.Vertex) isVinit (model: Model) =
    { model with graph = AAG.setVinit model.graph isVinit vertex.id }


// View
let private showFactor (vertex: AAG.Vertex) =
    Template.Vinit.Factor().Name(vertex.name).Elt()

let view (model: Model) dispatch =
    Template
        .Vinit()
        .Factors(forEach (AAG.getVinit model.graph) showFactor)
        .BackButton(fun _ -> dispatch Msg.OpenMainMenuView)
        .Elt()
