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

let openModifyAccess model = { model with step = ModifyAccess }

let private showAccess dispatch (access: AAG.Access) =
    Template
        .ModifyVertex
        .Access()
        .Name(access.name)
        .Button(fun _ -> dispatch Msg.ClickedModifyAccess)
        .Elt()

let isValidVertex model =
    (getVertexNameHint model).level <> Error

let handleClickedVertex (vertex: AAG.Vertex option) (model: Model) =
    if isValidVertex model then
        match vertex with
        | Some vertex ->
            { model with
                step = ModifyVertex
                subjectVertexId = Some vertex.id
                modifyVertexNameInput = vertex.name }
        | None -> model
    else
        model

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
                .AddAccessButton(fun _ -> dispatch Msg.ClickedAddAccess)
                .BackButton(fun _ -> dispatch Msg.ClickedBackFromSubjectVertex)
                .Elt()
        | None -> Template.ModifyVertex().Elt()
    | None -> Template.ModifyVertex().Elt()
