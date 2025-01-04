module AAG.Client.AAG

open System

type Vertex =
    { id: Guid
      name: string
      accesses: Access list }

and Factor =
    { id: Guid // equivalent to the edge id
      vertexId: Guid }
    static member Default vertexId =
        { id = Guid.NewGuid()
          vertexId = vertexId }

and Access =
    { id: Guid
      name: string
      factors: Factor Set
      isProvisional: bool }
    static member Provisional name factors =
        { id = Guid.NewGuid()
          name = name
          factors = factors |> Set.map Factor.Default
          isProvisional = true }

    static member Default name factors =
        { id = Guid.NewGuid()
          name = name
          factors = factors |> Set.map Factor.Default
          isProvisional = false }

and Graph =
    { vertices: Vertex list }
    static member Empty = { vertices = [] }

    static member Example =
        { vertices =
            [ { id = Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de1")
                name = "test"
                accesses = [] }
              { id = Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de2")
                name = "test2"
                accesses =
                  [ (Access.Default
                        "access1"
                        (Set
                            .empty
                            .Add(Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de1"))
                            .Add(Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de3")))) ] }
              { id = Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de3")
                name = "test222"
                accesses = [] } ] }

let findVertexById id graph =
    graph.vertices
    |> List.tryFind (fun vertex -> vertex.id = id)

let findVertexByName name graph =
    graph.vertices
    |> List.tryFind (fun vertex -> vertex.name = name)

let isVertexWithNameInGraph vertexName graph =
    Seq.contains
        vertexName
        (graph.vertices
         |> Seq.map (fun vertex -> vertex.name))

let isInvalidVertexName value = String.IsNullOrWhiteSpace(value)
let isInvalidAccessName value = String.IsNullOrWhiteSpace(value)

let addVertex vertexName graph =
    let vertex: Vertex =
        { id = Guid.NewGuid()
          name = vertexName
          accesses = [] }

    { graph with vertices = vertex :: graph.vertices }

let removeAllProvisionalAccesses graph =
    { graph with
        vertices =
            graph.vertices
            |> List.map (fun vertex ->
                { vertex with
                    accesses =
                        vertex.accesses
                        |> List.filter (fun access -> not access.isProvisional) }) }

let addAccessToVertex access vertex =
    { vertex with accesses = access :: vertex.accesses }

let addAccessToGraph vertexId access graph =
    { graph with
        vertices =
            graph.vertices
            |> List.map (fun vertex ->
                if vertex.id = vertexId then
                    addAccessToVertex access vertex
                else
                    vertex) }

let setProvisionalAccess vertexId name (factors: Guid Set) graph =
    if factors.IsEmpty then
        removeAllProvisionalAccesses graph
    else
        { graph with
            vertices =
                graph.vertices
                |> List.map (fun vertex ->
                    if vertex.id = vertexId then
                        { vertex with
                            accesses =
                                (vertex.accesses
                                 |> List.filter (fun access -> not access.isProvisional))
                                @ [ Access.Provisional name factors ] }
                    else
                        vertex) }

let isAccessPresentWithFactors vertexId factors graph =
    match findVertexById vertexId graph with
    | Some vertex ->
        vertex.accesses
        |> List.exists (fun access ->
            access.factors |> Set.map (fun factor -> factor.vertexId) = factors
            && access.isProvisional = false)
    | None -> false

let isAccessPresentWithName vertexId name graph =
    match findVertexById vertexId graph with
    | Some vertex ->
        vertex.accesses
        |> List.exists (fun access -> access.name = name && access.isProvisional = false)
    | None -> false
