module AAG.Client.AddAccessPage

open Model

let private getFactorsHint (model: Model) =
    match model.subjectId with
    | Some subjectId ->
        if model.factorsInput.IsEmpty then
            Hint.Error "Select at least one factor"
        elif AAG.isAccessPresentWithFactors subjectId model.factorsInput model.graph then
            Hint.Error "There is already an access with the same factors"
        else
            Hint.Info
    | None -> Hint.Error "No subject vertex selected yet"

let private getAccessNameHint (model: Model) =
    match model.subjectId with
    | Some subjectId ->
        if model.addAccessInput = "" then
            Hint.Required
        elif AAG.isInvalidAccessName model.addAccessInput then
            Hint.Error "Invalid name"
        elif AAG.isAccessPresentWithName subjectId model.addAccessInput model.graph then
            Hint.Error "Access name already taken for that vertex"
        else
            Hint.Info
    | None -> Hint.Error "Select a subject vertex first!"

let private getSubjectNameHint (model: Model) =
    if model.subjectInput = "" then
        Hint.Required
    else
        match AAG.findVertexByName model.subjectInput model.graph with
        | Some _ -> Hint.Info
        | None -> Hint.Error "Vertex does not exist"

let private deselectSubject (model: Model) =
    { model with
        subjectId = None
        subjectInput = ""
        graph = AAG.removeAllProvisionalAccesses model.graph }

let private selectSubject (vertex: AAG.Vertex) (model: Model) =
    { model with
        step = None
        subjectId = Some vertex.id
        graph =
            AAG.setProvisionalAccess
                vertex.id
                model.addAccessInput
                model.factorsInput
                (AAG.removeAllProvisionalAccesses model.graph)
        subjectInput = vertex.name }

let selectSubjectByName name (model: Model) =
    let vertex = AAG.findVertexByName name model.graph

    match vertex with
    | Some vertex -> selectSubject vertex model
    | None ->
        { model with
            subjectId = None
            subjectInput = name }

let toggleFactor (vertex: AAG.Vertex) (model: Model) =
    let factors =
        if Set.contains vertex.id model.factorsInput then
            Set.remove vertex.id model.factorsInput
        else
            Set.add vertex.id model.factorsInput

    match model.subjectId with
    | Some id ->
        { model with
            factorsInput = factors
            graph = AAG.setProvisionalAccess id model.addAccessInput factors model.graph }
    | None -> model

let cancel model =
    { model with
        subjectId = None
        subjectInput = ""
        factorsInput = Set.empty
        addAccessInput = ""
        graph = AAG.removeAllProvisionalAccesses model.graph
        page = Endpoint.MainMenu
        step = None }

let openFactorsSelection model = { model with step = Some "factors" }

let openSubjectSelection model = { model with step = None }

let handleClickedVertex (vertex: AAG.Vertex option) (model: Model) =
    match model.step with
    | None ->
        match vertex with
        | Some vertex -> selectSubject vertex model
        | None -> deselectSubject model
    | Some "factors" ->
        match vertex with
        | Some vertex -> toggleFactor vertex model
        | _ -> model
    | _ -> model

let private isSubjectValid (model: Model) =
    (getSubjectNameHint model).level <> Error
// TODO add checking name for save - also update temporary access / delete and add
let private isValidNewAccess (model: Model) =
    (getFactorsHint model).level <> Error
    && (getAccessNameHint model).level <> Error

let updateAccessName name (model: Model) = { model with addAccessInput = name }

let view jsRuntime (model: Model) dispatch =
    Utility.toggleButtonEnabled "ContinueButton" (isSubjectValid model) jsRuntime
    |> ignore

    Utility.toggleButtonEnabled "SaveButton" (isValidNewAccess model) jsRuntime
    |> ignore

    match model.step with
    | Some "factors" ->
        Template
            .AddAccessFactors()
            .BackButton(fun _ -> dispatch (Msg.ClickedBackFromFactors))
            .SaveButton(fun _ -> dispatch (Msg.ClickedSaveAddAccessButton))
            .AccessNameInput(model.addAccessInput, (fun v -> dispatch (Msg.TypedAccessName v)))
            .AccessNameHint((getAccessNameHint model).value) // TODO maybe use a function here instead of a raw value to a model
            .FactorsHint((getFactorsHint model).value)
            .Elt()
    | _ ->
        Template
            .AddAccessSubject()
            .CancelButton(fun _ -> dispatch Msg.ClickedCancelAddAccessButton)
            .ContinueButton(fun _ -> dispatch Msg.ClickedContinueSubject)
            .VertexNameInput(model.subjectInput, (fun v -> dispatch (Msg.TypedSubjectName v)))
            .VertexNameHint((getSubjectNameHint model).value)
            .Elt()
