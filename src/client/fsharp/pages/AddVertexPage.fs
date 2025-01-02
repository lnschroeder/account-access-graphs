module AAG.Client.AddVertexPage

open Model

let addVertex name (model: Model) =
    if AAG.isInvalidVertexName name then
        model
    elif AAG.isVertexWithNameInGraph name model.graph then
        model
    else
        { model with
            graph = AAG.addVertex name model.graph
            addVertexInput = Model.Init.addVertexInput }

let updateVertexName name (model: Model) =
    let hint =
        if name = "" then
            Input.AddVertexInput.hint
        elif AAG.isInvalidVertexName name then
            Hint.Error "Invalid name"
        elif AAG.isVertexWithNameInGraph name model.graph then
            Hint.Error "Vertex already exists"
        else
            Hint.Info

    { model with
        addVertexInput =
            { model.addVertexInput with
                value = name
                hint = hint } }

let cancel model =
    { model with
        addVertexInput = Input.AddVertexInput
        page = Endpoint.MainMenu }

let view jsRuntime (model: Model) dispatch =
    Utility.disableButton "SaveButton" (isInvalidInput model.addVertexInput) jsRuntime
    |> ignore

    Template
        .AddVertex()
        .CancelButton(fun _ -> dispatch Msg.ClickedCancelAddVertexButton)
        .SaveButton(fun _ -> dispatch (Msg.ClickedSaveAddVertexButton model.addVertexInput.value))
        .VertexNameInput(model.addVertexInput.value, (fun v -> dispatch (Msg.TypedVertexName v)))
        .VertexNameHint(model.addVertexInput.hint.value)
        .Elt()
