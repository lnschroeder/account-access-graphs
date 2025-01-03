module AAG.Client.VisJSTransformer

open System
open Model

type Node =
    { id: Guid
      label: string
      isSelected: bool
      isFactor: bool }

type Edge =
    { id: Guid
      from: Guid
      ``to``: Guid
      isProvisional: bool }

type Network = { nodes: Node list; edges: Edge list }

let private transformVertex model (vertex: AAG.Vertex) =
    { id = vertex.id
      label = vertex.name
      isSelected = vertex.name = model.subjectInput.value
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
                     { id = Guid.NewGuid()
                       from = factor
                       ``to`` = vertex.id
                       isProvisional = access.isProvisional })
                 |> Seq.toList))) }
