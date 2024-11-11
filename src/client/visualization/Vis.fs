module AAG.Client.Vis

open System

type VisNetworkNode = { id: Guid; label: string }

type VisNetworkEdge =
    { id: Guid
      from: string
      ``to``: string }

type VisNetwork =
    { nodes: VisNetworkNode list
      edges: VisNetworkEdge list }

let private node2VisNetworkNode (node: AAG.Node) = { id = node.id; label = node.name }

let graph2visNetwork (graph: AAG.Graph) =
    { nodes = (graph.nodes |> List.map node2VisNetworkNode)
      edges = [] }
