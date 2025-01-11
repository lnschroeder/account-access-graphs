module AAG.Client.VisJSTransformer

open System
open Model

type Node =
    { id: Guid
      label: string
      isSubject: bool
      isFactor: bool
      isInitiallyCompromised: bool
      isTransitivelyCompromised: bool }

type Edge =
    { id: Guid
      from: Guid
      ``to``: Guid
      isProvisional: bool
      isHighlighted: bool
      colorIndex: byte }

type Network = { nodes: Node list; edges: Edge list }

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
      isTransitivelyCompromised = isTransitivelyCompromised }

let private transformEdge (model: Model) (edge: AAG.Edge) =
    { id = edge.id
      from = edge.from
      ``to`` = edge.``to``
      isProvisional = Set.contains edge.id model.selectedFactors
      colorIndex = edge.colorIndex
      isHighlighted = Set.contains edge.id model.highlightedEdgeIds }
let transform (model: Model) =
    { nodes =
        (model.graph.vertices
         |> List.map (transformVertex model))
      edges = model.graph.edges |> List.map (transformEdge model) }
        // (model.graph.vertices
        //  |> List.collect (fun vertex ->
        //      vertex.accesses
        //      |> List.collect (fun access ->
        //          access.factors
        //          |> Set.map (fun factor ->
        //              { id = factor.id
        //                from = factor.vertexId
        //                ``to`` = vertex.id
        //                isProvisional =
        //                  (model.subjectAccessId = Some access.id
        //                   && (access.factors |> Set.map (fun f -> f.vertexId)) = model.selectedFactors)
        //                colorIndex = access.colorIndex
        //                isHighlighted = Some access.id = model.highlightedAccess })
        //          |> Seq.toList))) }
