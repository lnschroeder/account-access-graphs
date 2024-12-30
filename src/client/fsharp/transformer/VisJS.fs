module AAG.Client.VisJSTransformer

open System

type Node = { id: Guid; label: string }

type Edge =
    { id: Guid
      from: string
      ``to``: string }

type Network =
    { nodes: Node list
      edges: Edge list }

let private transformVertex (vertex: AAG.Vertex) = { id = vertex.id; label = vertex.name }

let transform (graph: AAG.Graph) =
    { nodes = (graph.vertices |> List.map transformVertex)
      edges = [] }
