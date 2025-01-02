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
let selectFactorById (idAsString: string) (model: Model) =
    // let hint =
    //     if name = "" then
    //         Input.VertexForNewAccessInput.hint
    //     elif not (AAG.isVertexWithNameInGraph name model.graph) then
    //         Hint.Error "Vertex does not exist"
    //     else
    //         Hint.Info
    let name =
        if idAsString = "undefined" then
            ""
        else
            let guid = Guid.Parse idAsString
            let vertex = AAG.findVertexById guid model.graph

            match vertex with
            | Some vertex -> vertex.name
            | None -> ""

    let factors =
        if List.contains name model.factorsInput then
            List.filter ((<>) name) model.factorsInput
        else
            name :: model.factorsInput

    { model with factorsInput = factors }

let cancel model =
    { deselectSubject model with
        page = Endpoint.MainMenu
        step = None }

let continueSubject model = { model with step = Some "factors" }

let handleClickedVertex (vertex: AAG.Vertex option) (model: Model) =
    if model.step = None then
        match vertex with
        | Some vertex -> selectSubject vertex model
        | None -> deselectSubject model
    else
        model

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
            .VertexNameInput(
                model.subjectInput.value,
                (fun v -> dispatch (Msg.TypedSubjectName v))
            )
            .VertexNameHint(model.subjectInput.hint.value)
            .Elt()
