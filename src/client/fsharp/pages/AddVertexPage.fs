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

let addNewVertex (model: Model) = // TODO replace with getVertexNameHint instead
    if AAG.isInvalidVertexName model.addVertexInput then
        model
    elif AAG.isVertexWithNameInGraph model.addVertexInput model.graph then
        model
    else
        { model with
            graph = AAG.addVertex model.addVertexInput model.graph
            addVertexInput = Model.Init.addVertexInput } // TODo use cancel instead

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
        .SaveButton(fun _ -> dispatch (Msg.ClickedSaveAddVertexButton))
        .VertexNameInput(model.addVertexInput, (fun v -> dispatch (Msg.TypedVertexName v)))
        .VertexNameHint((getVertexNameHint model).value)
        .Elt()
