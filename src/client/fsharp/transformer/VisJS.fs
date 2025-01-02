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
      from: string
      ``to``: string }

type Network = { nodes: Node list; edges: Edge list }

let private transformVertex model (vertex: AAG.Vertex) =
    { id = vertex.id
      label = vertex.name
      isSelected = vertex.name = model.subjectInput.value
      isFactor = Seq.contains vertex.name model.factorsInput }

let transform (model: Model) =
    { nodes =
        (model.graph.vertices
         |> List.map (transformVertex model))
      edges = [] }
