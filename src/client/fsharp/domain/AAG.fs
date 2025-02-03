module AAG.Client.AAG

open System

open System.Collections.Generic
open Newtonsoft.Json.Linq

type IdAccess = { target: Guid; factors: Guid Set }

type Rule = { description: string; logic: obj }

and Condition =
    { name: string
      description: string
      answer: bool }

and OptionalAccessMethod =
    { name: string
      description: string
      answer: bool }

and Component =
    { name: string
      conditions: Condition list
      rules: Rule list
      accessMethods: OptionalAccessMethod list
      graph: Graph }
    static member Empty =
        { name = ""
          conditions = []
          accessMethods = []
          rules = []
          graph = Graph.Empty }

and AccessSet =
    { factors: Guid Set }
    static member Singleton id = { factors = Set.singleton id }

and AccessBase =
    { accessSets: AccessSet Set }
    static member Singleton accessSet =
        { accessSets = Set.singleton accessSet }

    static member Empty = { accessSets = Set.empty }

and Vertex =
    { id: Guid
      name: string
      isVinit: bool
      score: int
      accessBase: AccessBase
      component_: string option }

    static member Default =
        { id = Guid.NewGuid()
          name = ""
          isVinit = false
          score = 1
          accessBase = AccessBase.Empty
          component_ = None }

and Edge =
    { id: Guid
      accessName: string
      colorIndex: byte
      disabled: bool
      from: Guid
      ``to``: Guid
      component_: string option }
    static member New (fromId: Guid) (access: Access) =
        { id = Guid.NewGuid()
          accessName = access.name
          colorIndex = access.colorIndex
          disabled = false
          from = fromId
          ``to`` = access.vertexId
          component_ = None }

and Access =
    { name: String
      colorIndex: byte
      vertexId: Guid }


and Graph =
    { vertices: Vertex list
      edges: Edge list
      components: Component list }
    static member Empty =
        { vertices = []
          edges = []
          components = [] }

    static member Example =

        let v1 =
            { id = Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de1")
              name = "test"
              isVinit = false
              score = 1
              accessBase = AccessBase.Empty
              component_ = None }

        let v2 =
            { id = Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de2")
              name = "test2"
              isVinit = false
              score = 2
              accessBase = AccessBase.Empty
              component_ = None }

        let v3 =
            { id = Guid.Parse("1578d946-7c48-41a6-baf2-0386979c9de3")
              name = "test222"
              isVinit = true
              score = 3
              accessBase = AccessBase.Empty
              component_ = None }

        let a1e1 =
            { id = Guid.Parse("2578d946-7c48-41a6-baf2-0386979c9de1")
              accessName = "access 1"
              colorIndex = 1uy
              disabled = false
              from = v1.id
              ``to`` = v2.id
              component_ = None }

        let a1e2 =
            { id = Guid.Parse("2578d946-7c48-41a6-baf2-0386979c9de2")
              accessName = "access 1"
              colorIndex = 1uy
              disabled = false
              from = v3.id
              ``to`` = v2.id
              component_ = None }

        let a2e1 =
            { id = Guid.Parse("2578d946-7c48-41a6-baf2-0386979c9de3")
              accessName = "access 2"
              colorIndex = 2uy
              disabled = false
              from = v3.id
              ``to`` = v2.id
              component_ = None }

        let a3e1 =
            { id = Guid.Parse("2578d946-7c48-41a6-baf2-0386979c9de4")
              accessName = "access 3"
              colorIndex = 3uy
              disabled = false
              from = v1.id
              ``to`` = v3.id
              component_ = None }

        { vertices = [ v1; v2; v3 ]
          edges = [ a1e1; a1e2; a2e1; a3e1 ]
          components = [] }

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

// TODO
let private isAccessFulfilled (vertexIds: Guid Set) (edges: Edge list) =
    Set.isSubset (edges |> List.map (fun e -> e.from) |> Set.ofList) vertexIds

let getVerticesWithName vertexName graph =
    graph.vertices
    |> Seq.filter (fun v -> v.name = vertexName)

let getComponentsWithName componentName graph =
    graph.vertices
    |> Seq.filter (fun v -> v.component_ = componentName)

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
                    { v with score = score }
                else
                    v) }
//
let getEnabledAccesses vertexId (graph: Graph) =
    graph.edges
    |> List.filter (fun e -> e.``to`` = vertexId && e.disabled = false) // TODO optimize
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
        getEnabledAccesses vertex.id graph
        |> Set.map (getEdgesForAccess graph)
        |> Set.exists (isAccessFulfilled vertexIds)

let getNextAvailableColor vertexId graph =
    let usedColors =
        getEnabledAccesses vertexId graph
        |> Set.map (fun a -> a.colorIndex)
        |> Seq.sort

    Seq.zip usedColors (Seq.initInfinite byte)
    |> Seq.tryFind (fun (a, b) -> a <> b)
    |> function
        | Some (_, b) -> b
        | None -> Seq.length usedColors |> byte

let getNextAvailableName vertexId graph =
    let usedNames =
        getEnabledAccesses vertexId graph
        |> Set.map (fun a -> a.name)
        |> Seq.sort

    Seq.zip usedNames (Seq.initInfinite int)
    |> Seq.tryFind (fun (a, b) -> a <> $"{b}")
    |> function
        | Some (_, b) -> $"{b}"
        | None -> $"{Seq.length usedNames |> int}"

let getAccessesWithFactors vertexId factors graph =
    getEnabledAccesses vertexId graph
    |> List.ofSeq
    |> List.filter (fun a ->
        ((getEdgesForAccess graph a)
         |> List.map (fun e -> e.from)
         |> Set.ofList) = factors)

let getAccessesWithName vertexId name graph =
    getEnabledAccesses vertexId graph
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

// AccessBase

let getScore graph (accessSet: AccessSet) =
    accessSet.factors
    |> Set.map (fun f ->
        tryFindVertexById f graph
        |> Option.map (fun v -> v.score)
        |> Option.defaultValue (Int32.MaxValue))
    |> List.ofSeq
    |> List.sum

let rec cart1 (LL) =
    match LL with
    | [] -> Seq.singleton []
    | L :: Ls ->
        seq {
            for x in L do
                for xs in cart1 Ls -> x :: xs
        }

let rec private computeAccessBaseStep
    (accesses: IdAccess list)
    (accessBases: Dictionary<Guid, AccessBase>)
    : Dictionary<Guid, AccessBase> =
    let mutable updated = false

    accesses
    |> List.iter (fun access ->
        let accessBasesOfFactors =
            access.factors
            |> Set.map (fun f -> accessBases.[f])
            |> List.ofSeq

        let accessSets =
            cart1 (
                accessBasesOfFactors
                |> List.map (fun accessBase -> accessBase.accessSets)
            )
            |> Seq.map (fun accessSets ->
                { factors =
                    accessSets
                    |> List.collect (fun accessSet -> accessSet.factors |> List.ofSeq)
                    |> Set.ofList })
            |> Set.ofSeq

        let oldAccessBaseSets = accessBases.[access.target].accessSets
        let newAccessBaseSets = (accessSets + oldAccessBaseSets)

        let newAccessBaseSets =
            newAccessBaseSets
            |> Set.filter (fun set1 ->
                not (
                    newAccessBaseSets
                    |> Set.exists (fun set2 ->
                        set1 <> set2
                        && Set.isSubset set2.factors set1.factors)
                ))

        let newAccessBase = { accessSets = newAccessBaseSets }

        updated <-
            updated
            || oldAccessBaseSets <> newAccessBase.accessSets

        accessBases.[access.target] <- newAccessBase)

    if updated then
        computeAccessBaseStep accesses accessBases
    else
        accessBases

let computeAccessBase graph : Graph =
    let accesses =
        graph.edges
        |> List.filter (fun e -> e.disabled = false)
        |> List.groupBy (fun e -> (e.``to``, e.colorIndex))
        |> List.map (fun ((vertexId, _), edges) ->
            { target = vertexId
              factors = edges |> Seq.map (fun e -> e.from) |> Set.ofSeq })

    let accessBases = Dictionary<Guid, AccessBase>()

    graph.vertices
    |> List.iter (fun v ->
        accessBases.[v.id] <-
            if v.isVinit then
                AccessBase.Singleton(AccessSet.Singleton v.id)
            else
                AccessBase.Empty)

    let accessBases = computeAccessBaseStep accesses accessBases

    { graph with
        vertices =
            graph.vertices
            |> List.map (fun v -> { v with accessBase = accessBases.[v.id] }) }

// component
let getComponent (graph: Graph) componentName =
    graph.components
    |> List.tryFind (fun c -> c.name = componentName)
    |> Option.defaultValue Component.Empty

let private updateVertexId graph oldId =
    let newId = Guid.NewGuid()

    { vertices =
        graph.vertices
        |> List.map (fun v ->
            if v.id = oldId then
                { v with id = newId }
            else
                v)
      edges =
        graph.edges
        |> List.map (fun e ->
            { e with
                from = if e.from = oldId then newId else e.from
                ``to`` =
                    if e.``to`` = oldId then
                        newId
                    else
                        e.``to`` })
      components = graph.components }

let rec private updateVertexIds graph vertexIds =
    match vertexIds with
    | [] -> graph
    | oldId :: rest ->
        let updatedGraph = updateVertexId graph oldId
        updateVertexIds updatedGraph rest

let private updateIds (graph: Graph) =
    let graph =
        { graph with
            edges =
                graph.edges
                |> List.map (fun e -> { e with id = Guid.NewGuid() }) }

    updateVertexIds graph (graph.vertices |> List.map (fun v -> v.id))

let setDisableEdgesByComponentNameAndAccessName graph componentName accessName disabled =
    { graph with
        edges =
            graph.edges
            |> List.map (fun e ->
                if e.accessName = accessName
                   && e.component_ = Some componentName then
                    { e with disabled = disabled }
                else
                    e)
        components =
            graph.components
            |> List.map (fun c ->
                { c with
                    accessMethods =
                        c.accessMethods
                        |> List.map (fun am ->
                            if am.name = accessName && c.name = componentName then
                                { am with answer = not disabled }
                            else
                                am) }) }

let setConditionByComponentNameAndConditionName graph componentName conditionName answer =
    { graph with
        components =
            graph.components
            |> List.map (fun component_ ->
                { component_ with
                    conditions =
                        component_.conditions
                        |> List.map (fun c ->
                            if c.name = conditionName
                               && component_.name = componentName then
                                { c with answer = answer }
                            else
                                c) }) }

let rec private disableAllEdgesForOptionalAccessMethods
    (graph: Graph)
    componentName
    (accessMethods: OptionalAccessMethod list)
    =
    match accessMethods with
    | [] -> graph
    | accessMethod :: accessMethods ->
        let updatedComponent =
            setDisableEdgesByComponentNameAndAccessName graph componentName accessMethod.name (not accessMethod.answer)

        disableAllEdgesForOptionalAccessMethods updatedComponent componentName accessMethods

let importComponent aag (aagc: Component) =
    let aagc =
        { aagc with
            graph =
                disableAllEdgesForOptionalAccessMethods
                    { aagc.graph with
                        vertices =
                            aagc.graph.vertices
                            |> List.map (fun v -> { v with component_ = Some aagc.name })
                        edges =
                            aagc.graph.edges
                            |> List.map (fun e -> { e with component_ = Some aagc.name }) }
                    aagc.name
                    aagc.accessMethods
                |> updateIds }

    { vertices = List.append aag.vertices aagc.graph.vertices
      edges = List.append aag.edges aagc.graph.edges
      components = List.append aag.components [ aagc ] }

let deleteComponent graph componentName =
    graph.vertices
    |> List.filter (fun v -> v.component_ = Some componentName)
    |> List.fold (fun g v -> removeVertexFromGraph v.id g) graph

// Authentication policy
open JsonLogic.Net

let private questionToJsonLogicData (name: string) (answer: bool) =
    let answer = if answer then "true" else "false"
    $""" "{name}": {answer} """

let private conditionToJsonLogicData (condition: Condition) =
    questionToJsonLogicData condition.name condition.answer

let private optionalAccessMethodToJsonLogicData (am: OptionalAccessMethod) =
    questionToJsonLogicData am.name am.answer


let evaluateAuthenticationPolicyRule (graph: Graph) componentName (rule: Rule): bool =
    let evaluator = JsonLogicEvaluator(EvaluateOperators.Default)
    let rule = JObject.Parse(rule.logic.ToString())
    let component_ =
        graph.components
        |> List.find (fun c -> c.name = componentName)

    let items =
        List.append
            (component_.conditions
             |> List.map conditionToJsonLogicData)
            (component_.accessMethods
             |> List.map optionalAccessMethodToJsonLogicData)

    let dataString = "{" + $"""{String.Join(",", items)}""" + "}"
    let dataObj = JObject.Parse(dataString)
    evaluator.Apply(rule, dataObj) :?> bool
