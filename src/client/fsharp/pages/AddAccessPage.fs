module AAG.Client.AddAccessPage

open Model

let selectVertexForNewAccess name (model: Model) =
    let hint =
        if name = "" then
            Input.VertexForNewAccessInput.hint
        elif not (AAG.isVertexWithNameInGraph name model.graph) then
            Hint.Error "Vertex does not exist"
        else
            Hint.Info

    { model with
        step = None
        vertexForNewAccessInput =
            { model.vertexForNewAccessInput with
                value = name
                hint = hint } }

let selectFactorForNewAccess name (model: Model) =
    // let hint =
    //     if name = "" then
    //         Input.VertexForNewAccessInput.hint
    //     elif not (AAG.isVertexWithNameInGraph name model.graph) then
    //         Hint.Error "Vertex does not exist"
    //     else
    //         Hint.Info
    let factors =
        if List.contains name model.factorsForNewAccessInput then
            List.filter ((<>) name) model.factorsForNewAccessInput
        else
            name :: model.factorsForNewAccessInput

    { model with factorsForNewAccessInput = factors }

let cancel model =
    { model with
        vertexForNewAccessInput = Input.VertexForNewAccessInput
        page = Endpoint.MainMenu
        step = None }

let continueSubject model = { model with step = Some "factors" }

let view jsRuntime (model: Model) dispatch =
    Utility.disableButton "ContinueButton" (isInvalidInput model.vertexForNewAccessInput) jsRuntime
    |> ignore

    match model.step with
    | Some "factors" ->
        Template
            .AddAccessFactors()
            .BackButton(fun _ -> dispatch (Msg.SelectVertexForNewAccess model.vertexForNewAccessInput.value))
            .Elt()
    | _ ->
        Template
            .AddAccessSubject()
            .CancelButton(fun _ -> dispatch Msg.CancelAddAccess)
            .ContinueButton(fun _ -> dispatch Msg.ContinueAddAccessSubject)
            .VertexNameInput(model.vertexForNewAccessInput.value, (fun v -> dispatch (Msg.SelectVertexForNewAccess v)))
            .VertexNameHint(model.vertexForNewAccessInput.hint.value)
            .Elt()
