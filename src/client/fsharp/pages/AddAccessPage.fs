module AAG.Client.AddAccessPage

open Model

let private getAccessNameHint name (model: Model) =
    match model.subjectId with
    | Some subjectId ->
        if name = "" then
            Input.AddAccessInput.hint
        elif AAG.isInvalidAccessName name then
            Hint.Error "Invalid name"
        elif AAG.isAccessPresentWithName subjectId name model.graph then
            Hint.Error "Access name already taken for that vertex"
        else
            Hint.Info
    | None -> Hint.Error "Select a subject vertex first!"

let private getUpdatedAccessNameInput (model: Model) =
    { model.addAccessInput with hint = getAccessNameHint model.addAccessInput.value model }

let private deselectSubject (model: Model) =
    { model with
        subjectId = None
        subjectInput = Input.SubjectInput
        addAccessInput = getUpdatedAccessNameInput model
        graph = AAG.removeAllProvisionalAccesses model.graph }

let private selectSubject (vertex: AAG.Vertex) (model: Model) =
    { model with
        step = None
        subjectId = Some vertex.id
        graph =
            AAG.setProvisionalAccess
                vertex.id
                model.addAccessInput.value
                model.factorsInput
                (AAG.removeAllProvisionalAccesses model.graph)
        addAccessInput = getUpdatedAccessNameInput model
        subjectInput =
            { value = vertex.name
              hint = Hint.Info } }

let selectSubjectByName name (model: Model) =
    let vertex = AAG.findVertexByName name model.graph

    match vertex with
    | Some vertex -> selectSubject vertex model
    | None ->
        { model with
            subjectId = None
            subjectInput =
                { value = name
                  hint = Hint.Error "Vertex does not exist" }
            addAccessInput = getUpdatedAccessNameInput model }

let toggleFactor (vertex: AAG.Vertex) (model: Model) =
    let factors =
        if Set.contains vertex.id model.factorsInput then
            Set.remove vertex.id model.factorsInput
        else
            Set.add vertex.id model.factorsInput

    match model.subjectId with
    | Some id ->
        { model with
            addAccessInput = getUpdatedAccessNameInput model
            factorsInput = factors
            graph = AAG.setProvisionalAccess id model.addAccessInput.value factors model.graph }
    | None -> model

let cancel model =
    { model with
        subjectId = None
        subjectInput = Input.SubjectInput
        factorsInput = Set.empty
        addAccessInput = Input.AddAccessInput
        graph = AAG.removeAllProvisionalAccesses model.graph
        page = Endpoint.MainMenu
        step = None }

let openFactorsSelection model =
    { model with
        step = Some "factors"
        addAccessInput = getUpdatedAccessNameInput model }

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

let private isSubjectValid (model: Model) = model.subjectId.IsSome
// TODO add checking name for save - also update temporary access / delete and add
let private isValidNewAccess (model: Model) =
    match model.subjectId with
    | Some subjectId ->
        not model.factorsInput.IsEmpty
        && not (AAG.isAccessPresentWithFactors subjectId model.factorsInput model.graph)
        && isValidInput model.addAccessInput
    | None -> false

let updateAccessName name (model: Model) =
    { model with
        addAccessInput =
            { model.addAccessInput with
                value = name
                hint = getAccessNameHint name model } }

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
            .AccessNameInput(model.addAccessInput.value, (fun v -> dispatch (Msg.TypedAccessName v)))
            .AccessNameHint(model.addAccessInput.hint.value)
            .Elt()
    | _ ->
        Template
            .AddAccessSubject()
            .CancelButton(fun _ -> dispatch Msg.ClickedCancelAddAccessButton)
            .ContinueButton(fun _ -> dispatch Msg.ClickedContinueSubject)
            .VertexNameInput(model.subjectInput.value, (fun v -> dispatch (Msg.TypedSubjectName v)))
            .VertexNameHint(model.subjectInput.hint.value)
            .Elt()
