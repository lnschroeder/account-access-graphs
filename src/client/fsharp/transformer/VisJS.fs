module AAG.Client.VisJSTransformer

open System
open Model

type Node =
    { id: Guid
      label: string
      isSubject: bool
      isFactor: bool }

type Edge =
    { id: Guid
      from: Guid
      ``to``: Guid
      isProvisional: bool
      colorIndex: byte }

type Network = { nodes: Node list; edges: Edge list }

let private transformVertex model (vertex: AAG.Vertex) =
    { id = vertex.id
      label = vertex.name
      isSubject = vertex.name = model.subjectInput
      isFactor = Seq.contains vertex.id model.factorsInput }

let transform (model: Model) =
    { nodes =
        (model.graph.vertices
         |> List.map (transformVertex model))
      edges =
        (model.graph.vertices
         |> List.collect (fun vertex ->
             vertex.accesses
             |> List.collect (fun access ->
                 access.factors
                 |> Set.map (fun factor ->
                     { id = factor.id
                       from = factor.vertexId
                       ``to`` = vertex.id
                       isProvisional = access.isProvisional
                       colorIndex = access.colorIndex })
                 |> Seq.toList))) }
