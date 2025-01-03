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

let updateVertexName name (model: Model) = { model with addVertexInput = name }

let cancel model =
    { model with
        addVertexInput = ""
        page = Endpoint.MainMenu }

let isValidNewVertex model =
    (getVertexNameHint model).level <> Error

let addNewVertex (model: Model) =
    if isValidNewVertex model then
        let graph =  AAG.addVertex model.addVertexInput model.graph
        cancel { model with graph = graph }
    else
        model // TODO

let view jsRuntime (model: Model) dispatch =
    Utility.toggleButtonEnabled "SaveButton" (isValidNewVertex model) jsRuntime
    |> ignore

    Template
        .AddVertex()
        .CancelButton(fun _ -> dispatch Msg.ClickedCancelAddVertexButton)
        .SaveButton(fun _ -> dispatch (Msg.ClickedSaveAddVertexButton))
        .VertexNameInput(model.addVertexInput, (fun v -> dispatch (Msg.TypedVertexName v)))
        .VertexNameHint((getVertexNameHint model).value)
        .Elt()
