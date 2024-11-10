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

let isNodeNameInGraph value graph =
    Seq.contains value (graph.nodes |> Seq.map (fun node -> node.name))

let isInvalidNodeName value = String.IsNullOrWhiteSpace(value)

let addNode name graph =
    let newNode: Node =
        { id = Guid.NewGuid()
          name = name
          accesses = [] }

    { graph with nodes = newNode :: graph.nodes }
