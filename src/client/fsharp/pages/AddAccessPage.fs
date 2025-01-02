module AAG.Client.AddAccessPage

open Model
open System

let private deselectSubject (model: Model) =
    { model with subjectInput = Input.SubjectInput }

let private selectSubject (vertex: AAG.Vertex) (model: Model) =
    { model with
        step = None
        subjectInput =
            { value = vertex.name
              hint = Hint.Info } }

let selectSubjectByName name (model: Model) =
    let vertex = AAG.findVertexByName name model.graph

    match vertex with
    | Some vertex -> selectSubject vertex model
    | None ->
        { model with
            subjectInput =
                { value = name
                  hint = Hint.Error "Vertex does not exist" } }

let toggleFactor (vertex: AAG.Vertex) (model: Model) =
    let factors =
        if List.contains vertex.name model.factorsInput then
            List.filter ((<>) vertex.name) model.factorsInput
        else
            vertex.name :: model.factorsInput

    { model with factorsInput = factors }

let cancel model =
    { model with
        subjectInput = Input.SubjectInput
        factorsInput = []
        page = Endpoint.MainMenu
        step = None }

let continueSubject model = { model with step = Some "factors" }

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

let view jsRuntime (model: Model) dispatch =
    Utility.disableButton "ContinueButton" (isInvalidInput model.subjectInput) jsRuntime
    |> ignore

    match model.step with
    | Some "factors" ->
        Template
            .AddAccessFactors()
            .BackButton(fun _ -> dispatch (Msg.TypedSubjectName model.subjectInput.value))
            .Elt()
    | _ ->
        Template
            .AddAccessSubject()
            .CancelButton(fun _ -> dispatch Msg.ClickedCancelAddAccessButton)
            .ContinueButton(fun _ -> dispatch Msg.ClickedContinueSubject)
            .VertexNameInput(model.subjectInput.value, (fun v -> dispatch (Msg.TypedSubjectName v)))
            .VertexNameHint(model.subjectInput.hint.value)
            .Elt()
