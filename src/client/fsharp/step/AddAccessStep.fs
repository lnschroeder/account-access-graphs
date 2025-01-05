module AAG.Client.AddAccessStep

open Model
open Bolero.Html

let private getFactorsHint (model: Model) = // TODO just pass minimal
    match model.subjectId with
    | Some subjectId ->
        if model.factorsInput.IsEmpty then
            Hint.Error "Select at least one factor"
        elif Seq.contains subjectId model.factorsInput then
            Hint.Error "Self-references are not allowed"
        elif AAG.isAccessPresentWithFactors subjectId (Set.ofList model.factorsInput) model.graph then
            Hint.Error "There is already an access with the same factors"
        else
            Hint.Info
    | None -> Hint.Error "No subject vertex selected yet"

let private getAccessNameHint (model: Model) =
    match model.subjectId with
    | Some subjectId ->
        if model.addAccessNameInput = "" then
            Hint.Info
        elif AAG.isInvalidAccessName model.addAccessNameInput then
            Hint.Error "Invalid name"
        elif AAG.isAccessPresentWithName subjectId model.addAccessNameInput model.graph then
            Hint.Error "Access name already taken for that vertex"
        else
            Hint.Info
    | None -> Hint.Error "Select a subject vertex first!"

let private getAccessNameInputPlaceholder (model: Model) =
    match model.subjectId with
    | Some subjectId ->
        match AAG.findVertexById subjectId model.graph with
        | Some subject ->
            "Defaults to: "
            + (AAG.findNextAvailableColor subject).ToString()
        | None -> "Invalid subject vertex selected"
    | None -> "Select a subject vertex first"

let private getSubjectNameHint (model: Model) =
    if model.subjectInput = "" then
        Hint.Required
    else
        match AAG.findVertexByName model.subjectInput model.graph with
        | Some vertex ->
            if Seq.contains vertex.id model.factorsInput then
                Hint.Error "Self-references are not allowed"
            else
                Hint.Info
        | None -> Hint.Error "Vertex does not exist"

let private deselectSubject (model: Model) =
    { model with
        subjectId = None
        subjectInput = ""
        graph = AAG.removeAllProvisionalAccesses model.graph }

let private selectSubject (vertex: AAG.Vertex) (model: Model) =
    { model with
        subjectId = Some vertex.id
        graph =
            AAG.setProvisionalAccess
                vertex.id
                model.addAccessNameInput
                (Set.ofList model.factorsInput)
                (AAG.removeAllProvisionalAccesses model.graph)
        subjectInput = vertex.name }

let selectSubjectByName name (model: Model) =
    let vertex = AAG.findVertexByName name model.graph

    match vertex with
    | Some vertex -> selectSubject vertex model
    | None ->
        { model with
            subjectId = None
            subjectInput = name
            graph = AAG.removeAllProvisionalAccesses model.graph }

let removeProvisionalFactor vertexId (model: Model) =
    match model.subjectId with
    | Some subjectId ->
        let factors =
            model.factorsInput
            |> List.filter (fun id -> id <> vertexId)

        { model with
            factorsInput = factors
            graph = AAG.setProvisionalAccess subjectId model.addAccessNameInput (Set.ofList factors) model.graph }
    | None -> model

let addProvisionalFactor vertexId (model: Model) =
    match model.subjectId with
    | Some subjectId ->
        let factors = model.factorsInput @ [ vertexId ]

        { model with
            factorsInput = factors
            graph = AAG.setProvisionalAccess subjectId model.addAccessNameInput (Set.ofList factors) model.graph }
    | None -> model

let toggleFactor vertexId (model: Model) =
    if Some vertexId = model.subjectId then
        model
    elif List.contains vertexId model.factorsInput then
        removeProvisionalFactor vertexId model
    else
        addProvisionalFactor vertexId model

let cancel model =
    { model with
        subjectId = None
        subjectInput = ""
        factorsInput = []
        addAccessNameInput = ""
        graph = AAG.removeAllProvisionalAccesses model.graph
        step = Main }

let openFactorsSelection model = { model with step = AddAccessFactors }

let openSubjectSelection model = { model with step = AddAccessSubject }

let handleClickedVertexOnSubject (vertex: AAG.Vertex option) (model: Model) =
    match vertex with
    | Some vertex -> selectSubject vertex model
    | None -> deselectSubject model

let handleClickedVertexOnFactors (vertex: AAG.Vertex option) (model: Model) =
    match vertex with
    | Some vertex -> toggleFactor vertex.id model
    | _ -> model

let private isSubjectValid (model: Model) =
    (getSubjectNameHint model).level <> Error

let private isValidNewAccess (model: Model) =
    (getFactorsHint model).level <> Error
    && (getAccessNameHint model).level <> Error

let updateAccessName name (model: Model) =
    { model with addAccessNameInput = name }

let addNewAccess name (model: Model) =
    match model.subjectId with
    | Some subjectId ->
        match AAG.findVertexById subjectId model.graph with
        | Some vertex ->
            let access: AAG.Access =
                AAG.Access.Default name (Set.ofList model.factorsInput) (AAG.findNextAvailableColor vertex)

            let graph =
                AAG.addAccessToGraph subjectId access (AAG.removeAllProvisionalAccesses model.graph) // TODO make more efficient

            cancel { model with graph = graph }
        | None -> model
    | _ -> model // TODO

let private showFactor (model: Model) dispatch id =
    match AAG.findVertexById id model.graph with
    | Some vertex ->
        Template
            .AddAccessFactors
            .Factor()
            .Name(vertex.name)
            .DeleteButton(fun _ -> dispatch (Msg.ClickedRemoveProvisionalFactor vertex.id))
            .Elt()
    | None ->
        Template
            .AddAccessFactors
            .Factor()
            .Name("INVALID")
            .DeleteButton(fun _ -> ())
            .Elt()

let view jsRuntime (model: Model) dispatch =
    Utility.toggleButtonEnabled "ContinueButton" (isSubjectValid model) jsRuntime
    |> ignore

    Utility.toggleButtonEnabled "SaveButton" (isValidNewAccess model) jsRuntime
    |> ignore

    match model.step with
    | AddAccessFactors ->
        Template
            .AddAccessFactors()
            .BackButton(fun _ -> dispatch (Msg.ClickedBackFromFactors))
            .SaveButton(fun _ ->
                dispatch (
                    Msg.ClickedSaveAddAccess(
                        if model.subjectInput = "" then
                            None
                        else
                            Some model.subjectInput
                    )
                ))
            .AccessNameInput(model.addAccessNameInput, (fun v -> dispatch (Msg.TypedAccessName v)))
            .AccessNameInputPlaceholder(getAccessNameInputPlaceholder model)
            .AccessNameHint((getAccessNameHint model).value)
            .Factors(forEach model.factorsInput (showFactor model dispatch))
            .FactorsHint((getFactorsHint model).value)
            .Elt()
    | _ ->
        Template
            .AddAccessSubject()
            .CancelButton(fun _ -> dispatch Msg.ClickedCancelAddAccess)
            .ContinueButton(fun _ -> dispatch Msg.ClickedContinueSubject)
            .VertexNameInput(model.subjectInput, (fun v -> dispatch (Msg.TypedSubjectName v)))
            .VertexNameHint((getSubjectNameHint model).value)
            .Elt()
