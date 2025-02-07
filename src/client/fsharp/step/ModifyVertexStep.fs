module AAG.Client.ModifyVertexStep

open Model
open Bolero.Html

// Validity
let isValidVertex model = isValid model modifyVertexNameInput

let isSubjectVertexEmpty (model: Model) =
    model.subjectVertexId
    |> Option.map (fun id ->
        let vertex = AAG.tryFindVertexById id model.graph

        match vertex with
        | Some v ->
            v.name = ""
            && (model.graph.edges
                |> List.filter (fun e -> e.from = id || e.``to`` = id))
                .IsEmpty
        | None -> true)
    |> Option.defaultValue true

// Handle actions
let handleClickedVertex (vertex: AAG.Vertex) (model: Model) dispatch =
    if isValidVertex model then
        dispatch (Msg.OpenModifyVertexStep(Some vertex.id))
    else
        dispatch Msg.IgnoreAction

let handleClickedBackground (model: Model) dispatch =
    if isValidVertex model then
        dispatch (Msg.OpenMainMenuStep)
    elif isSubjectVertexEmpty model then
        dispatch (
            match model.subjectVertexId with
            | Some subjectVertexId -> Msg.ClickedDeleteVertex subjectVertexId
            | None -> Msg.IgnoreAction
        )
    else
        dispatch Msg.IgnoreAction

let handleClickedEdge (access: AAG.Access) (model: Model) dispatch =
    if isValidVertex model then
        dispatch (Msg.OpenModifyAccessStep(access, access.name))
    else
        dispatch Msg.IgnoreAction

// Open
let private openInternal (vertex: AAG.Vertex) model =
    { page = model.page
      step = ModifyVertex
      physics = model.physics
      edgeLabels = model.edgeLabels
      graph = model.graph
      addAccessNameInput = ""
      modifyVertexNameInput = vertex.name
      subjectVertexId = Some vertex.id
      subjectAccessColor = None
      subjectAccessName = ""
      selectedFactors = Set.empty
      highlightedEdgeIds = Set.empty
      initiallyCompromisedVertexIds = Set.empty
      transitivelyCompromisedVertexIds = Set.empty
      newComponentSelection = ""
      components = []
      componentNameInput = ""
      subjectComponentName = ""
      json = model.json }

let ``open`` vertexId model =
    match vertexId with
    | Some vertexId ->
        match AAG.tryFindVertexById vertexId model.graph with
        | Some vertex -> openInternal vertex model
        | None -> model // TODO throw error if vertexId is not found
    | None ->
        let vertex = AAG.Vertex.Default
        let graph = AAG.addVertex vertex model.graph
        openInternal vertex { model with graph = graph }

// Functionality
let updateVertexName subjectVertexId name (model: Model) =
    { model with
        modifyVertexNameInput = name
        graph = AAG.updateVertexName subjectVertexId name model.graph }

let exitDeletingVertex vertexId (model: Model) =
    { model with
        step = MainMenu
        modifyVertexNameInput = ""
        graph = AAG.removeVertexFromGraph vertexId model.graph }

let setScore (vertex: AAG.Vertex) score (model: Model) =
    { model with graph = AAG.setScore model.graph (max 0 score) vertex.id }

// View
let private showFactor graph vertexId =
    Template
        .ModifyVertex
        .FactorName()
        .Name(
            AAG.tryFindVertexById vertexId graph
            |> Option.map (fun v -> v.name)
            |> Option.defaultValue "INVALID"
        ).Elt()

let private showAccess dispatch graph (access: AAG.Access) =
    Template
        .ModifyVertex
        .Access()
        .Name(access.name)
        .FactorNames(
            forEach
                (AAG.getEdgesForAccess graph access
                 |> List.map (fun e -> e.from)
                 |> Set.ofList)
                (showFactor graph)
        )
        .Button(fun _ -> dispatch (Msg.OpenModifyAccessStep(access, access.name)))
        .Enter(fun _ -> dispatch (Msg.HighlightAccess(access)))
        .Leave(fun _ -> dispatch (Msg.DeHighlightAccess))
        .Elt()

let view jsRuntime (model: Model) dispatch =
    Utility.toggleButtonEnabled "ModifyVertexBackButton" (isValidVertex model) jsRuntime
    |> ignore

    match model.subjectVertexId with
    | Some subjectVertexId ->
        match AAG.tryFindVertexById subjectVertexId model.graph with
        | Some subjectVertex ->
            Utility.toggleButtonEnabled "ModifyVertexDeleteButton" (subjectVertex.component_.IsNone) jsRuntime
            |> ignore
            Template
                .ModifyVertex()
                .SubjectNameInput(
                    model.modifyVertexNameInput,
                    (fun v -> dispatch (Msg.ModifiedVertexName(subjectVertexId, v)))
                )
                .SubjectNameHint((modifyVertexNameInput model).value)
                .Accesses(
                    forEach (AAG.getEnabledAccesses subjectVertex.id model.graph) (showAccess dispatch model.graph)
                )
                .AddAccessButton(fun _ ->
                    dispatch (
                        Msg.OpenModifyAccessStep(
                            { vertexId = subjectVertexId
                              colorIndex = AAG.getNextAvailableColor subjectVertex.id model.graph
                              name = AAG.getNextAvailableName subjectVertex.id model.graph },
                            ""
                        )
                    ))
                .VinitInput(
                    subjectVertex.isVinit,
                    (fun b ->
                        if b then
                            dispatch (Msg.SetVinit subjectVertex)
                        else
                            dispatch (Msg.UnsetVinit subjectVertex))
                )
                .ScoreInput(subjectVertex.score, (fun i -> dispatch (Msg.SetScore(subjectVertex, i))))
                .ComponentInfo(
                    match subjectVertex.component_ with
                    | Some c ->
                        Template
                            .ModifyVertex
                            .SomeComponentInfo()
                            .ComponentName(c)
                            .ModifyComponentButton(fun _ -> dispatch (Msg.OpenModifyComponent c))
                            .Elt()
                    | None -> Template.ModifyVertex.NoneComponentInfo().Elt()
                )
                .BackButton(fun _ -> dispatch Msg.OpenMainMenuStep)
                .DeleteButton(fun _ -> dispatch (Msg.ClickedDeleteVertex subjectVertexId))
                .Elt()
        | None -> Template.ModifyVertex().Elt()
    | None -> Template.ModifyVertex().Elt()
