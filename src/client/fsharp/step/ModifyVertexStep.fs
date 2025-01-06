module AAG.Client.ModifyVertexStep

open Model
open Bolero.Html

let private getVertexNameHint (model: Model) =
    let name = model.modifyVertexNameInput

    if name = "" then
        Hint.Required
    elif AAG.isInvalidVertexName name then
        Hint.Error "Invalid name"
    elif Seq.length (AAG.getVerticesWithName name model.graph) > 1 then
        Hint.Error "Vertex already exists"
    else
        Hint.Info

let updateVertexName subjectVertexId name (model: Model) =
    { model with
        modifyVertexNameInput = name
        graph = AAG.changeVertexName subjectVertexId name model.graph }

let private openInternal (vertex: AAG.Vertex) model =
    { page = model.page
      step = ModifyVertex
      graph = model.graph
      addAccessNameInput = model.addAccessNameInput
      modifyVertexNameInput = vertex.name
      subjectVertexId = Some vertex.id
      subjectAccessId = model.subjectAccessId
      factorsInput = [] }

let ``open`` vertexId model =
    match vertexId with
    | Some vertexId ->
        match AAG.findVertexById vertexId model.graph with
        | Some vertex -> openInternal vertex model
        | None -> model // TODO throw error if vertexId is not found
    | None ->
        let vertex = AAG.Vertex.Default
        let graph = AAG.addVertex vertex model.graph
        openInternal vertex { model with graph = graph }

let private showAccess dispatch (access: AAG.Access) =
    Template
        .ModifyVertex
        .Access()
        .Name(access.name)
        .Button(fun _ -> dispatch (Msg.OpenModifyAccessStep(Some access.id)))
        .Elt()

let isValidVertex model =
    (getVertexNameHint model).level <> Error

let handleClickedVertex (vertex: AAG.Vertex option) (model: Model) dispatch =
    if isValidVertex model then
        match vertex with
        | Some vertex -> dispatch (Msg.OpenModifyVertexStep(Some vertex.id))
        | None -> dispatch (Msg.OpenMainMenuStep)
    else
        dispatch Msg.IgnoreAction

let saveAndExit (model: Model) =
    { model with
        modifyVertexNameInput = ""
        subjectVertexId = None
        step = MainMenu }

let view jsRuntime (model: Model) dispatch =
    Utility.toggleButtonEnabled "ModifyVertexBackButton" (isValidVertex model) jsRuntime
    |> ignore

    match model.subjectVertexId with
    | Some subjectVertexId ->
        match AAG.findVertexById subjectVertexId model.graph with
        | Some subject ->
            Template
                .ModifyVertex()
                .SubjectNameInput(
                    model.modifyVertexNameInput,
                    (fun v -> dispatch (Msg.ModifiedVertexName(subjectVertexId, v)))
                )
                .SubjectNameHint((getVertexNameHint model).value)
                .Accesses(forEach subject.accesses (showAccess dispatch))
                .AddAccessButton(fun _ -> dispatch (Msg.OpenModifyAccessStep None))
                .BackButton(fun _ -> dispatch Msg.ClickedBackFromSubjectVertex)
                .Elt()
        | None -> Template.ModifyVertex().Elt()
    | None -> Template.ModifyVertex().Elt()
