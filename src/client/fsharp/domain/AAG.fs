module AAG.Client.AAG

open System

type Vertex =
    { id: Guid
      name: string
      accesses: Access list }
    static member Default =
        { id = Guid.NewGuid()
          name = ""
          accesses = [] }

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
      colorIndex: byte
      isProvisional: bool }
    static member Provisional name factors =
        { id = Guid.NewGuid()
          name = name
          factors = factors |> Set.map Factor.Default
          isProvisional = true
          colorIndex = Byte.MaxValue }

    static member Default name factors colorIndex =
        { id = Guid.NewGuid()
          name =
            if name = "" then
                colorIndex.ToString()
            else
                name
          factors = factors |> Set.map Factor.Default
          isProvisional = false
          colorIndex = colorIndex }

    static member New colorIndex =
        { id = Guid.NewGuid()
          name = colorIndex.ToString()
          factors = Set.empty
          isProvisional = false
          colorIndex = colorIndex }

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
                        ""
                        (Set
                            .empty
                            .Add(Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de1"))
                            .Add(Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de3")))
                        1uy)
                    (Access.Default "" (Set.empty.Add(Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de3"))) 11uy) ] }
              { id = Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de3")
                name = "test222"
                accesses = [] } ] }

let findNextAvailableColor (vertex: Vertex) =
    let usedColors =
        vertex.accesses
        |> Seq.map (fun access -> access.colorIndex)
        |> Seq.sort

    Seq.zip usedColors (Seq.initInfinite byte)
    |> Seq.tryFind (fun (a, b) -> a <> b)
    |> function
        | Some (_, b) -> b
        | None -> Seq.length usedColors |> byte

let rec findInVertices vertices accessId =
    match vertices with
    | [] -> None
    | vertex :: rest ->
        match List.tryFind (fun access -> access.id = accessId) vertex.accesses with
        | Some access -> Some(access, vertex)
        | None -> findInVertices rest accessId

// let findAccessById (accessId: Guid) (graph: Graph) : Access option * Vertex option =
//     match findInVertices graph.vertices accessId with
//     | Some (access, vertex) -> (Some access, Some vertex)
//     | None -> (None, None)

let private addFactorToAccess vertexId (access: Access) =
    { access with factors = Set.add (Factor.Default vertexId) access.factors }

let private removeFactorFromAccess vertexId (access: Access) =
    { access with
        factors =
            access.factors
            |> Set.filter (fun factor -> factor.vertexId <> vertexId) }

let addFactorToGraph accessId vertexId graph =
    { graph with
        vertices =
            graph.vertices
            |> List.map (fun vertex ->
                { vertex with
                    accesses =
                        (vertex.accesses
                         |> List.map (fun access ->
                             if access.id = accessId then
                                 addFactorToAccess vertexId access
                             else
                                 access)) }) }

let removeFactorFromGraph accessId vertexId graph =
    { graph with
        vertices =
            graph.vertices
            |> List.map (fun vertex ->
                { vertex with
                    accesses =
                        (vertex.accesses
                         |> List.map (fun access ->
                             if access.id = accessId then
                                 removeFactorFromAccess vertexId access
                             else
                                 access)) }) }

let findVertexById id graph = // TODO change id to option
    graph.vertices
    |> List.tryFind (fun vertex -> vertex.id = id)

let findAccessById accessId graph = // TODO change id to option
    graph.vertices
    |> List.tryPick (fun vertex ->
        vertex.accesses
        |> List.tryFind (fun access -> access.id = accessId)
        |> Option.map (fun access -> access, vertex))

let getVerticesWithName vertexName graph =
    graph.vertices
    |> Seq.filter (fun vertex -> vertex.name = vertexName)

let isInvalidVertexName value = String.IsNullOrWhiteSpace(value)
let isInvalidAccessName value = String.IsNullOrWhiteSpace(value)

let addVertex vertex graph =
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

// let setProvisionalAccess vertexId name (factors: Guid Set) graph =
//     if factors.IsEmpty then
//         removeAllProvisionalAccesses graph
//     else
//         { graph with
//             vertices =
//                 graph.vertices
//                 |> List.map (fun vertex ->
//                     if vertex.id = vertexId then
//                         { vertex with
//                             accesses =
//                                 (vertex.accesses
//                                  |> List.filter (fun access -> not access.isProvisional))
//                                 @ [ Access.Provisional name factors ] }
//                     else
//                         vertex) }

let deleteAccess accessId graph =
    { graph with
        vertices =
            graph.vertices
            |> List.map (fun v ->
                { v with
                    accesses =
                        v.accesses
                        |> List.filter (fun a -> a.id <> accessId) }) }

let getAccessesWithFactors vertexId factors graph = // TODO rename factors to vertices?!
    match findVertexById vertexId graph with
    | Some vertex ->
        vertex.accesses
        |> Seq.filter (fun access ->
            (access.factors
             |> Seq.map (fun factor -> factor.vertexId)) = factors)
    | None -> Seq.empty

let getAccessesWithName vertexId name graph =
    match findVertexById vertexId graph with
    | Some vertex ->
        vertex.accesses
        |> Seq.filter (fun access -> access.name = name)
    | None -> Seq.empty

let changeVertexName vertexId name graph =
    { graph with
        vertices =
            graph.vertices
            |> List.map (fun vertex ->
                if vertex.id = vertexId then
                    { vertex with name = name }
                else
                    vertex) }

let changeAccessName accessId name graph =
    { graph with
        vertices =
            graph.vertices
            |> List.map (fun vertex ->
                { vertex with
                    accesses =
                        vertex.accesses
                        |> List.map (fun access ->
                            if access.id = accessId then
                                { access with name = name }
                            else
                                access) }) }
