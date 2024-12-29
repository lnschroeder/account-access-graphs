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

let isNodeWithNameInGraph nodeName graph =
    Seq.contains nodeName (graph.nodes |> Seq.map (fun node -> node.name))

let isInvalidNodeName value = String.IsNullOrWhiteSpace(value)

let addNode nodeName graph =
    let newNode: Node =
        { id = Guid.NewGuid()
          name = nodeName
          accesses = [] }

    { graph with nodes = newNode :: graph.nodes }
