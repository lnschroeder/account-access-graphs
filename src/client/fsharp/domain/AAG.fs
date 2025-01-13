module AAG.Client.AAG

open System

type Vertex =
    { id: Guid
      name: string
      isVinit: bool
      score: int
      _score: int }
    static member Default =
        { id = Guid.NewGuid()
          name = ""
          isVinit = false
          score = 1
          _score = 1 }

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

and Access =
    { name: String
      colorIndex: byte
      vertexId: Guid }


and Graph =
    { vertices: Vertex list
      edges: Edge list }
    static member Empty = { vertices = []; edges = [] }

    static member Example =

        let v1 =
            { id = Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de1")
              name = "test"
              isVinit = false
              score = 1
              _score = 1 }

        let v2 =
            { id = Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de2")
              name = "test2"
              isVinit = false
              score = 2
              _score = 2 }

        let v3 =
            { id = Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de3")
              name = "test222"
              isVinit = true
              score = 3
              _score = 3 }

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

let private transformEdgeToAccess (edge: Edge) =
    { name = edge.accessName
      colorIndex = edge.colorIndex
      vertexId = edge.``to`` }

let addVertex vertex graph =
    { graph with vertices = vertex :: graph.vertices }

let addEdge edge graph =
    { graph with edges = edge :: graph.edges }

let private isEdgeInAccess (access: Access) (edge: Edge) =
    edge.accessName = access.name
    && edge.``to`` = access.vertexId
    && edge.colorIndex = access.colorIndex

let private isAccessFulfilled (vertexIds: Guid Set) (edges: Edge list) =
    Set.isSubset (edges |> List.map (fun e -> e.from) |> Set.ofList) vertexIds

let getVerticesWithName vertexName graph =
    graph.vertices
    |> Seq.filter (fun v -> v.name = vertexName)

let tryFindVertexById id graph =
    graph.vertices
    |> List.tryFind (fun v -> v.id = id)

let updateVertexName vertexId name graph =
    { graph with
        vertices =
            graph.vertices
            |> List.map (fun v ->
                if v.id = vertexId then
                    { v with name = name }
                else
                    v) }

let removeVertexFromGraph vertexId graph =
    { graph with
        vertices =
            graph.vertices
            |> List.filter (fun v -> v.id <> vertexId)
        edges =
            graph.edges
            |> List.filter (fun e -> e.from <> vertexId && e.``to`` <> vertexId) }

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

let getVinit graph =
    graph.vertices |> List.filter (fun v -> v.isVinit)

let setVinit graph isVinit vertexId =
    { graph with
        vertices =
            graph.vertices
            |> List.map (fun v ->
                if v.id = vertexId then
                    { v with isVinit = isVinit }
                else
                    v) }

let setScore graph score vertexId =
    { graph with
        vertices =
            graph.vertices
            |> List.map (fun v ->
                if v.id = vertexId then
                    { v with score = score; _score = score }
                else
                    v) }
//
let getAccesses vertexId (graph: Graph) =
    graph.edges
    |> List.filter (fun e -> e.``to`` = vertexId)
    |> List.map transformEdgeToAccess
    |> Set.ofList

let getEdgesForAccess graph (access: Access) =
    graph.edges |> List.filter (isEdgeInAccess access)

let tryFindAccessByEdgeId id graph =
    graph.edges
    |> List.tryFind (fun e -> e.id = id)
    |> Option.map transformEdgeToAccess

let tryFindAccessByVertexIdAndColor vertexId color graph =
    graph.edges
    |> List.tryFind (fun e -> e.``to`` = vertexId && e.colorIndex = color)
    |> Option.map transformEdgeToAccess

let updateAccessName (access: Access) name graph =
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

let removeAccessFromGraph access graph =
    { graph with
        edges =
            graph.edges
            |> List.filter (fun e -> transformEdgeToAccess e <> access) }

//

let private isAccessible (graph: Graph) (vertexIds: Guid Set) (vertex: Vertex) =
    match vertex with
    | vertex when Set.contains vertex.id vertexIds -> true
    | _ ->
        getAccesses vertex.id graph
        |> Set.map (getEdgesForAccess graph)
        |> Set.exists (isAccessFulfilled vertexIds)

let getNextAvailableColor vertexId graph =
    let usedColors =
        getAccesses vertexId graph
        |> Set.map (fun a -> a.colorIndex)
        |> Seq.sort

    Seq.zip usedColors (Seq.initInfinite byte)
    |> Seq.tryFind (fun (a, b) -> a <> b)
    |> function
        | Some (_, b) -> b
        | None -> Seq.length usedColors |> byte

let getNextAvailableName vertexId graph =
    let usedNames =
        getAccesses vertexId graph
        |> Set.map (fun a -> a.name)
        |> Seq.sort

    Seq.zip usedNames (Seq.initInfinite int)
    |> Seq.tryFind (fun (a, b) -> a <> $"{b}")
    |> function
        | Some (_, b) -> $"{b}"
        | None -> $"{Seq.length usedNames |> int}"

let getAccessesWithFactors vertexId factors graph =
    getAccesses vertexId graph
    |> List.ofSeq
    |> List.filter (fun a ->
        ((getEdgesForAccess graph a)
         |> List.map (fun e -> e.from)
         |> Set.ofList) = factors)

let getAccessesWithName vertexId name graph =
    getAccesses vertexId graph
    |> Set.filter (fun a -> a.name = name)

//
let private getNewlyCompromisedVertexId
    (compromisedVertexIds: Guid Set)
    (graph: Graph)
    (vertices: Vertex list)
    : Vertex option =
    vertices
    |> List.tryFind (isAccessible graph compromisedVertexIds)

//
let rec private getCompromisedVertices (compromisedVertexIds: Guid Set) (graph: Graph) (vertices: Vertex list) =
    let vertices =
        vertices
        |> List.filter (fun v -> not <| Set.contains v.id compromisedVertexIds)

    match getNewlyCompromisedVertexId compromisedVertexIds graph vertices with
    | None -> compromisedVertexIds
    | Some v -> getCompromisedVertices (Set.add v.id compromisedVertexIds) graph vertices

//
let getCompromisedVerticesOfGraph (compromisedVertexIds: Guid Set) (graph: Graph) =
    getCompromisedVertices compromisedVertexIds graph graph.vertices

////
let private reset_Score vertex = { vertex with _score = vertex.score }

let resetScores graph =
    { graph with vertices = graph.vertices |> List.map reset_Score }

let private tryFindFactorForEdge graph (edge: Edge) = tryFindVertexById edge.from graph

let private tryFindFactorsForAccess (graph: Graph) (access: Access) =
    (getEdgesForAccess graph access)
    |> List.map (tryFindFactorForEdge graph)

let get_Score (vertex: Vertex option) =
    vertex
    |> Option.map (fun v -> v._score)
    |> Option.defaultValue Int32.MinValue

let sum_Scores (vertices: Vertex option list) =
    List.sum (vertices |> List.map get_Score)

let private recomputeSumThenMinScore graph vertexId =
    let _scores =
        getAccesses vertexId graph
        |> Set.map (tryFindFactorsForAccess graph)
        |> Set.map sum_Scores

    if _scores.IsEmpty then
        Int32.MaxValue
    else
        Set.minElement _scores

let private stepRecomputeSumThenMinScore graph =
    { graph with
        vertices =
            graph.vertices
            |> List.map (fun v -> { v with _score = min v._score (recomputeSumThenMinScore graph v.id) }) }

let rec recomputeSumThenMinScores graph =
    let newGraph = stepRecomputeSumThenMinScore graph

    if newGraph = graph then
        newGraph
    else
        recomputeSumThenMinScores newGraph
