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

let private transformNode (node: AAG.Node) = { id = node.id; label = node.name }

let transform (graph: AAG.Graph) =
    { nodes = (graph.nodes |> List.map transformNode)
      edges = [] }
