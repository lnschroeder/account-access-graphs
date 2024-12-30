module AAG.Client.AAG

open System

type Vertex =
    { id: Guid
      name: string
      accesses: Access list }

and Access = { access: string list }

and Graph =
    { vertices: Vertex list }
    static member Empty = { vertices = [] }

let isVertexWithNameInGraph vertexName graph =
    Seq.contains vertexName (graph.vertices |> Seq.map (fun vertex -> vertex.name))

let isInvalidVertexName value = String.IsNullOrWhiteSpace(value)

let addVertex vertexName graph =
    let newVertex: Vertex =
        { id = Guid.NewGuid()
          name = vertexName
          accesses = [] }

    { graph with vertices = newVertex :: graph.vertices }
