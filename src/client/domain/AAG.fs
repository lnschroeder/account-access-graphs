module AAG.Client.AAG

open System

type Node =
    { id: Guid
      name: string
      accesses: Access list }

and Access = { access: string list }

and Graph =
    { nodes: Node list }
    static member Empty = { nodes = [] }

let isNodeNameInGraph (graph: Graph) value =
    Seq.contains value (graph.nodes |> Seq.map (fun node -> node.name))

let isInvalidNodeName graph value =
    String.IsNullOrWhiteSpace(value)
    || (isNodeNameInGraph graph value)
