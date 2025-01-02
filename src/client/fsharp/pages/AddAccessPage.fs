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
        vertexForNewAccessInput =
            { model.vertexForNewAccessInput with
                value = name
                hint = hint } }

let cancel model =
    { model with
        vertexForNewAccessInput = Input.VertexForNewAccessInput
        page = Endpoint.MainMenu }

let view jsRuntime (model: Model) dispatch =
    Utility.disableButton "ContinueButton" (isInvalidInput model.vertexForNewAccessInput) jsRuntime
    |> ignore

    Template
        .AddAccessSubject()
        .CancelButton(fun _ -> dispatch Msg.CancelAddAccess)
        .VertexNameInput(model.vertexForNewAccessInput.value, (fun v -> dispatch (Msg.SelectVertexForNewAccess v)))
        .VertexNameHint(model.vertexForNewAccessInput.hint.value)
        .Elt()
