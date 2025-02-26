module AAG.Client.VisJSTransformer

open System
open Model

type Node =
    { id: Guid
      label: string
      isSubject: bool
      isFactor: bool
      isInitiallyCompromised: bool
      isTransitivelyCompromised: bool
      isVinit: bool
      hasBackdoor: bool
      score: int
      component_: string }

type Edge =
    { id: Guid
      from: Guid
      ``to``: Guid
      isProvisional: bool
      isHighlighted: bool
      isDisabled: bool
      label: string
      colorIndex: byte
      straight: bool }

type Network =
    { nodes: Node list
      edges: Edge list
      view: string
      physics: bool
      edgeLabels: bool }

let private transformVertex (model: Model) (vertex: AAG.Vertex) =
    let isInitiallyCompromised =
        Set.contains vertex.id model.initiallyCompromisedVertexIds

    let isTransitivelyCompromised =
        Set.contains vertex.id model.transitivelyCompromisedVertexIds
        && not isInitiallyCompromised

    { id = vertex.id
      label = vertex.name
      isSubject =
        Some vertex.id = model.subjectVertexId
        || model.view = ModifyComponent
           && vertex.component_ = Some model.subjectComponentName
      isFactor = Seq.contains vertex.id model.selectedFactors
      isInitiallyCompromised = isInitiallyCompromised
      isTransitivelyCompromised = isTransitivelyCompromised
      isVinit = vertex.isVinit
      score = vertex.score
      hasBackdoor = (scoreHint vertex).level = Error
      component_ = vertex.component_ |> Option.defaultValue "" }

let private transformEdge (model: Model) (edge: AAG.Edge) =
    { id = edge.id
      from = edge.from
      ``to`` = edge.``to``
      label = edge.accessName
      isProvisional =
        model.subjectVertexId = Some edge.``to``
        && model.subjectAccessColor = Some edge.colorIndex
      colorIndex = edge.colorIndex
      isDisabled = edge.disabled
      isHighlighted = Set.contains edge.id model.highlightedEdgeIds
      straight =
        not model.physics
        && model.graph.edges
           |> List.filter (fun e -> e.from = edge.from && e.``to`` = edge.``to``)
           |> List.length = 1 }

let transform (model: Model) =
    { nodes =
        model.graph.vertices
        |> List.map (transformVertex model)
      edges =
        model.graph.edges
        |> List.map (transformEdge model)
      view = model.view.ToString()
      physics = model.physics
      edgeLabels = model.edgeLabels }
