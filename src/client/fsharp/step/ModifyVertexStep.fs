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

let updateVertexName subjectId name (model: Model) =
    { model with
        modifyVertexNameInput = name
        graph = AAG.changeVertexName subjectId name model.graph }

let openFactorsSelection model = { model with step = AddAccessFactors }

let private showAccess dispatch (access: AAG.Access) =
    Template
        .ModifyVertex
        .Access()
        .Name(access.name)
        .Button(fun _ -> dispatch Msg.ClickedModifyAccess)
        .Elt()

let isValidVertex model =
    (getVertexNameHint model).level <> Error

let saveAndExit (model: Model) =
    { model with
        modifyVertexNameInput = ""
        subjectId = None
        step = MainMenu }

let view jsRuntime (model: Model) dispatch =
    Utility.toggleButtonEnabled "ModifyVertexSaveButton" (isValidVertex model) jsRuntime
    |> ignore

    match model.subjectId with
    | Some subjectId ->
        match AAG.findVertexById subjectId model.graph with
        | Some subject ->
            Template
                .ModifyVertex()
                .SubjectNameInput(
                    model.modifyVertexNameInput,
                    (fun v -> dispatch (Msg.ModifiedVertexName(subjectId, v)))
                )
                .SubjectNameHint((getVertexNameHint model).value)
                .Accesses(forEach subject.accesses (showAccess dispatch))
                .AddAccessButton(fun _ -> dispatch Msg.ClickedAddAccess)
                .SaveButton(fun _ -> dispatch Msg.ClickedSaveSubjectVertex)
                .Elt()
        | None -> Template.ModifyVertex().Elt()
    | None -> Template.ModifyVertex().Elt()
