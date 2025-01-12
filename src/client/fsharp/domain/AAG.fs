module AAG.Client.AAG

open System

type Vertex =
    { id: Guid
      name: string }
    static member Default = { id = Guid.NewGuid(); name = "" }

and Access =
    { name: String
      colorIndex: byte
      vertexId: Guid }

and Edge =
    { id: Guid
      accessName: string
      colorIndex: byte
      from: Guid
      ``to``: Guid }

    static member New (fromId: Guid) (access: Access) =
        { id = Guid.NewGuid()
          accessName = access.name
          colorIndex = access.colorIndex
          from = fromId
          ``to`` = access.vertexId }


and Graph =
    { vertices: Vertex list
      edges: Edge list }
    static member Empty = { vertices = []; edges = [] }

    static member Example =

        let v1 =
            { id = Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de1")
              name = "test" }

        let v2 =
            { id = Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de2")
              name = "test2" }

        let v3 =
            { id = Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de3")
              name = "test222" }

        let a1e1 =
            { id = Guid.Parse("2578d946-7c48-41a6-baf2-0386979c9de1")
              accessName = "access 1"
              colorIndex = 1uy
              from = v1.id
              ``to`` = v2.id }

        let a1e2 =
            { id = Guid.Parse("2578d946-7c48-41a6-baf2-0386979c9de2")
              accessName = "access 1"
              colorIndex = 1uy
              from = v3.id
              ``to`` = v2.id }

        let a2e1 =
            { id = Guid.Parse("2578d946-7c48-41a6-baf2-0386979c9de3")
              accessName = "access 2"
              colorIndex = 2uy
              from = v3.id
              ``to`` = v2.id }

        let a3e1 =
            { id = Guid.Parse("2578d946-7c48-41a6-baf2-0386979c9de4")
              accessName = "access 3"
              colorIndex = 3uy
              from = v1.id
              ``to`` = v3.id }

        { vertices = [ v1; v2; v3 ]
          edges = [ a1e1; a1e2; a2e1; a3e1 ] }

// let private hasVertexId id (a: Vertex) = a.id = id

// let private hasFactorVertexId vertexId (a: Factor) = a.vertexId = vertexId

// let private removeFactorWithVertexIdFromFactors vertexId factors =
//     factors
//     |> Set.filter (not << hasFactorVertexId vertexId)

// let private removeVertexFactorFromAccess vertexId access =
//     { access with factors = (removeFactorWithVertexIdFromFactors vertexId access.factors) }

// let private removeVertexFactorFromAccesses vertexId accesses =
//     accesses
//     |> List.map (removeVertexFactorFromAccess vertexId)

// let private removeVertexFactorFromVertex vertexId vertex =
//     { vertex with
//         accesses =
//             removeVertexFactorFromAccesses vertexId vertex.accesses
//             |> List.filter (fun access -> not access.factors.IsEmpty) }

let removeVertexFromGraph vertexId graph =
    { graph with
        vertices =
            graph.vertices
            |> List.filter (fun v -> v.id <> vertexId)
        edges =
            graph.edges
            |> List.filter (fun e -> e.from <> vertexId && e.``to`` <> vertexId) }

// let isAccessFulfilled (vertexIds: Guid Set) (access: Access) =
//     Set.isSubset (access.factors |> Set.map (fun f -> f.vertexId)) vertexIds

// let canBeCompromised (vertexIds: Guid Set) (vertex: Vertex) =
//     match vertex with
//     | vertex when Set.contains vertex.id vertexIds -> true
//     | _ ->
//         vertex.accesses
//         |> List.exists (isAccessFulfilled vertexIds)

// let private getNewlyCompromisedVertexId (compromisedVertexIds: Guid Set) (vertices: Vertex list) : Vertex option =
//     vertices
//     |> List.tryFind (canBeCompromised compromisedVertexIds)

// let rec getCompromisedVertices (compromisedVertexIds: Guid Set) (vertices: Vertex list) =
//     let vertices =
//         vertices
//         |> List.filter (fun v -> not <| Set.contains v.id compromisedVertexIds)

//     match getNewlyCompromisedVertexId compromisedVertexIds vertices with
//     | None -> compromisedVertexIds
//     | Some v -> getCompromisedVertices (Set.add v.id compromisedVertexIds) vertices

// let getCompromisedVerticesOfGraph (compromisedVertexIds: Guid Set) (graph: Graph) =
//     getCompromisedVertices compromisedVertexIds graph.vertices

let private mapEdgeToAccess (edge: Edge) =
    { name = edge.accessName
      colorIndex = edge.colorIndex
      vertexId = edge.``to`` }

let getAccesses vertexId (graph: Graph) =
    graph.edges
    |> List.filter (fun e -> e.``to`` = vertexId)
    |> List.map mapEdgeToAccess
    |> Set.ofList

let private isEdgeInAccess (access: Access) (edge: Edge) =
    edge.accessName = access.name
    && edge.``to`` = access.vertexId
    && edge.colorIndex = access.colorIndex

let findNextAvailableColor vertexId graph =
    let usedColors =
        getAccesses vertexId graph
        |> Set.map (fun a -> a.colorIndex)
        |> Seq.sort

    Seq.zip usedColors (Seq.initInfinite byte)
    |> Seq.tryFind (fun (a, b) -> a <> b)
    |> function
        | Some (_, b) -> b
        | None -> Seq.length usedColors |> byte

let findNextAvailableName vertexId graph =
    let usedNames =
        getAccesses vertexId graph
        |> Set.map (fun a -> a.name)
        |> Seq.sort

    Seq.zip usedNames (Seq.initInfinite int)
    |> Seq.tryFind (fun (a, b) -> a <> $"{b}")
    |> function
        | Some (_, b) -> $"{b}"
        | None -> $"{Seq.length usedNames |> int}"

// let rec findInVertices vertices accessId =
//     match vertices with
//     | [] -> None
//     | vertex :: rest ->
//         match List.tryFind (fun access -> access.id = accessId) vertex.accesses with
//         | Some access -> Some(access, vertex)
//         | None -> findInVertices rest accessId

// let private addFactorToAccess vertexId (access: Access) =
//     { access with factors = Set.add (Factor.Default vertexId) access.factors }

// let private removeFactorFromAccess vertexId (access: Access) =
//     { access with
//         factors =
//             access.factors
//             |> Set.filter (fun factor -> factor.vertexId <> vertexId) }

// let addFactorToGraph accessId vertexId graph =
//     { graph with
//         vertices =
//             graph.vertices
//             |> List.map (fun vertex ->
//                 { vertex with
//                     accesses =
//                         (vertex.accesses
//                          |> List.map (fun access ->
//                              if access.id = accessId then
//                                  addFactorToAccess vertexId access
//                              else
//                                  access)) }) }

// let removeFactorFromGraph accessId vertexId graph =
//     { graph with
//         vertices =
//             graph.vertices
//             |> List.map (fun vertex ->
//                 { vertex with
//                     accesses =
//                         (vertex.accesses
//                          |> List.map (fun access ->
//                              if access.id = accessId then
//                                  removeFactorFromAccess vertexId access
//                              else
//                                  access)) }) }

let findVertexById id graph = // TODO change id to option
    graph.vertices
    |> List.tryFind (fun vertex -> vertex.id = id)

let findAccessByEdgeId id graph =
    graph.edges
    |> List.tryFind (fun e -> e.id = id)
    |> Option.map mapEdgeToAccess

let findAccessByVertexIdAndColor vertexId color graph =
    graph.edges
    |> List.tryFind (fun e -> e.``to`` = vertexId && e.colorIndex = color)
    |> Option.map mapEdgeToAccess

// let findAccessById accessId graph = // TODO change id to option
//     graph.vertices
//     |> List.tryPick (fun vertex ->
//         vertex.accesses
//         |> List.tryFind (fun access -> access.id = accessId)
//         |> Option.map (fun access -> vertex.id, access))

let findEdgesOfAccess graph (access: Access) = // TODO change id to option
    graph.edges |> List.filter (isEdgeInAccess access)

// graph.vertices
// |> List.tryPick (fun vertex ->
//     vertex.accesses
//     |> List.tryFind (fun access ->
//         access.factors
//         |> Seq.exists (fun factor -> factor.id = edgeId))
//     |> Option.map (fun access -> vertex.id, access))

let getVerticesWithName vertexName graph =
    graph.vertices
    |> Seq.filter (fun vertex -> vertex.name = vertexName)

let isInvalidVertexName value = String.IsNullOrWhiteSpace(value)

let isInvalidAccessName value = String.IsNullOrWhiteSpace(value)

let addVertex vertex graph =
    { graph with vertices = vertex :: graph.vertices }

let addEdge edge graph =
    { graph with edges = edge :: graph.edges }

let removeVertexFromAccess vertexId (access: Access) graph =
    { graph with
        edges =
            graph.edges
            |> List.filter (fun e ->
                not (
                    e.colorIndex = access.colorIndex
                    && e.from = vertexId
                    && e.``to`` = access.vertexId
                )) }

// let addAccessToVertex access vertex =
//     { vertex with accesses = access :: vertex.accesses }

// let addAccessToGraph vertexId access graph =
//     { graph with
//         vertices =
//             graph.vertices
//             |> List.map (fun vertex ->
//                 if vertex.id = vertexId then
//                     addAccessToVertex access vertex
//                 else
//                     vertex) }

let deleteAccess access graph =
    { graph with
        edges =
            graph.edges
            |> List.filter (fun e -> mapEdgeToAccess e <> access) }

let getAccessesWithFactors vertexId factors graph = // TODO rename factors to vertices?!
    getAccesses vertexId graph
    |> List.ofSeq
    |> List.map (fun a ->
        (findEdgesOfAccess graph a)
        |> List.map (fun e -> e.from)
        |> Set.ofList)
    // |> Set.map (Set.ofList)
    |> List.filter (fun fs -> fs = factors)

// | Some vertex ->
//     vertex.accesses
//     |> Seq.filter (fun access ->
//         (access.factors
//          |> Set.map (fun factor -> factor.vertexId)) = factors)
// | None -> Set.empty

let getAccessesWithName vertexId name graph =
    getAccesses vertexId graph
    |> Set.filter (fun a -> a.name = name)

// match findVertexById vertexId graph with
// | Some vertex ->
//     vertex.accesses
//     |> Seq.filter (fun access -> access.name = name)
// | None -> Seq.empty

let changeVertexName vertexId name graph =
    { graph with
        vertices =
            graph.vertices
            |> List.map (fun vertex ->
                if vertex.id = vertexId then
                    { vertex with name = name }
                else
                    vertex) }

let changeAccessName (access: Access) name graph =
    { graph with
        edges =
            graph.edges
            |> List.map (fun e ->
                { e with
                    accessName =
                        if isEdgeInAccess access e then
                            name
                        else
                            e.accessName }) }
