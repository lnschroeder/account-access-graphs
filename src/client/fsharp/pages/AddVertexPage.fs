module AAG.Client.AddVertexPage

open Model

let private getVertexNameHint (model: Model) =
    if model.addVertexInput = "" then
        Hint.Required
    elif AAG.isInvalidVertexName model.addVertexInput then
        Hint.Error "Invalid name"
    elif AAG.isVertexWithNameInGraph model.addVertexInput model.graph then
        Hint.Error "Vertex already exists"
    else
        Hint.Info

let addVertex name (model: Model) =
    if AAG.isInvalidVertexName name then
        model
    elif AAG.isVertexWithNameInGraph name model.graph then
        model
    else
        { model with
            graph = AAG.addVertex name model.graph
            addVertexInput = Model.Init.addVertexInput }

let updateVertexName name (model: Model) = { model with addVertexInput = name }

let cancel model =
    { model with
        addVertexInput = ""
        page = Endpoint.MainMenu }

let view jsRuntime (model: Model) dispatch =
    Utility.toggleButtonEnabled "SaveButton" ((getVertexNameHint model).level <> Error) jsRuntime
    |> ignore

    Template
        .AddVertex()
        .CancelButton(fun _ -> dispatch Msg.ClickedCancelAddVertexButton)
        .SaveButton(fun _ -> dispatch (Msg.ClickedSaveAddVertexButton model.addVertexInput))
        .VertexNameInput(model.addVertexInput, (fun v -> dispatch (Msg.TypedVertexName v)))
        .VertexNameHint((getVertexNameHint model).value)
        .Elt()
