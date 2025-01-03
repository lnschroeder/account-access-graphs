module AAG.Client.AddAccessPage

open Model

let private deselectSubject (model: Model) =
    { model with
        subjectId = None
        subjectInput = Input.SubjectInput
        graph = AAG.removeAllProvisionalAccesses model.graph }

let private selectSubject (vertex: AAG.Vertex) (model: Model) =
    { model with
        step = None
        subjectId = Some vertex.id
        graph = AAG.setProvisionalAccess vertex.id model.factorsInput (AAG.removeAllProvisionalAccesses model.graph)
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
                  hint = Hint.Error "Vertex does not exist" } }

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
            graph = AAG.setProvisionalAccess id factors model.graph }
    | None -> model

let cancel model =
    { model with
        subjectId = None
        subjectInput = Input.SubjectInput
        factorsInput = Set.empty
        graph = AAG.removeAllProvisionalAccesses model.graph
        page = Endpoint.MainMenu
        step = None }

let continueSubject model = { model with step = Some "factors" }

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

let private isValidNewAccess (model: Model) =
    match model.subjectId with
    | Some subjectId ->
        not model.factorsInput.IsEmpty
        && not (AAG.isAccessPresentWithFactors subjectId model.factorsInput model.graph)
    | None -> false

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
            .Elt()
    | _ ->
        Template
            .AddAccessSubject()
            .CancelButton(fun _ -> dispatch Msg.ClickedCancelAddAccessButton)
            .ContinueButton(fun _ -> dispatch Msg.ClickedContinueSubject)
            .VertexNameInput(model.subjectInput.value, (fun v -> dispatch (Msg.TypedSubjectName v)))
            .VertexNameHint(model.subjectInput.hint.value)
            .Elt()
