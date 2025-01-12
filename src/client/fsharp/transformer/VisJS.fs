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
      score: int }

type Edge =
    { id: Guid
      from: Guid
      ``to``: Guid
      isProvisional: bool
      isHighlighted: bool
      colorIndex: byte }

type Network =
    { nodes: Node list
      edges: Edge list
      step: string
      subjectVertexId: Guid option
     }

let private transformVertex (model: Model) (vertex: AAG.Vertex) =
    let isInitiallyCompromised =
        Set.contains vertex.id model.initiallyCompromisedVertexIds

    let isTransitivelyCompromised =
        Set.contains vertex.id model.transitivelyCompromisedVertexIds
        && not isInitiallyCompromised

    { id = vertex.id
      label = vertex.name
      isSubject = Some vertex.id = model.subjectVertexId
      isFactor = Seq.contains vertex.id model.selectedFactors
      isInitiallyCompromised = isInitiallyCompromised
      isTransitivelyCompromised = isTransitivelyCompromised
      isVinit = vertex.isVinit
      score = vertex.score }

let private transformEdge (model: Model) (edge: AAG.Edge) =
    { id = edge.id
      from = edge.from
      ``to`` = edge.``to``
      isProvisional =
        model.subjectVertexId = Some edge.``to``
        && model.subjectAccessColor = Some edge.colorIndex
      colorIndex = edge.colorIndex
      isHighlighted = Set.contains edge.id model.highlightedEdgeIds }

let transform (model: Model) =
    { nodes =
        (model.graph.vertices
         |> List.map (transformVertex model))
      edges =
        model.graph.edges
        |> List.map (transformEdge model)
      step = model.step.ToString()
      subjectVertexId = model.subjectVertexId
    }
