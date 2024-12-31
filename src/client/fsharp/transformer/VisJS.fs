module AAG.Client.VisJSTransformer

open System
open Model

type Node = { id: Guid; label: string }

type Edge =
    { id: Guid
      from: string
      ``to``: string }

type Network =
    { nodes: Node list
      edges: Edge list
      nodeIdsOfNewAccess: Guid option }

let private transformVertex (vertex: AAG.Vertex) = { id = vertex.id; label = vertex.name }

let transform (model: Model) =
    { nodes = (model.graph.vertices |> List.map transformVertex)
      edges = []
      nodeIdsOfNewAccess =
        match AAG.getVertexIdByName model.selectedVertexForNewAccess model.graph with
        | Some id -> Some id
        | None -> None
      }
