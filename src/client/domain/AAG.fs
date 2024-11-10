module AAG.Client.AAG

open System

type Node =
    { id: Guid
      name: string
      accesses: Access [] }

and Access = { access: string [] }

and Graph = { nodes: Node [] }

let isNodeNameInGraph (graph: Graph) value =
    Seq.contains value (graph.nodes |> Seq.map (fun node -> node.name))

let isInvalidNodeName graph value =
    String.IsNullOrWhiteSpace(value)
    || (isNodeNameInGraph graph value)
