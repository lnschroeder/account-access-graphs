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
      colorIndex: byte }

    static member New name colorIndex =
        { id = Guid.NewGuid()
          name = name
          factors = Set.empty
          colorIndex = colorIndex }

and Graph =
    { vertices: Vertex list }
    static member Empty = { vertices = [] }

    static member Example =
        let a1 =
            { id = Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de1")
              name = "access 1"
              factors =
                Set
                    .empty
                    .Add(Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de1"))
                    .Add(Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de3"))
                |> Set.map Factor.Default
              colorIndex = 1uy }

        let a2 =
            { id = Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de2")
              name = "access 2"
              factors =
                Set.empty.Add(Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de3"))
                |> Set.map Factor.Default
              colorIndex = 2uy }

        let v1 =
            { id = Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de1")
              name = "test"
              accesses = [] }

        let v2 =
            { id = Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de2")
              name = "test2"
              accesses = [ a1; a2 ] }

        let v3 =
            { id = Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de3")
              name = "test222"
              accesses = [] }

        { vertices = [ v1; v2; v3 ] }

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

let findNextAvailableName (vertex: Vertex) =
    let usedNames =
        vertex.accesses
        |> Seq.map (fun access -> access.name)
        |> Seq.sort

    Seq.zip usedNames (Seq.initInfinite int)
    |> Seq.tryFind (fun (a, b) -> a <> $"{b}")
    |> function
        | Some (_, b) -> $"{b}"
        | None -> $"{Seq.length usedNames |> int}"

let rec findInVertices vertices accessId =
    match vertices with
    | [] -> None
    | vertex :: rest ->
        match List.tryFind (fun access -> access.id = accessId) vertex.accesses with
        | Some access -> Some(access, vertex)
        | None -> findInVertices rest accessId

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
        |> Option.map (fun access -> vertex.id, access))

let getVerticesWithName vertexName graph =
    graph.vertices
    |> Seq.filter (fun vertex -> vertex.name = vertexName)

let isInvalidVertexName value = String.IsNullOrWhiteSpace(value)

let isInvalidAccessName value = String.IsNullOrWhiteSpace(value)

let addVertex vertex graph =
    { graph with vertices = vertex :: graph.vertices }

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
             |> Set.map (fun factor -> factor.vertexId)) = factors)
    | None -> Set.empty

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
