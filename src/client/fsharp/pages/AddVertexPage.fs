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
            newVertexName = "" }

let updateVertexName name (model: Model) =
    let error =
        if AAG.isInvalidVertexName name then
            Some "invalid vertex name"
        elif AAG.isVertexWithNameInGraph name model.graph then
            Some "vertex already exists"
        else
            None

    { model with
        newVertexName = name
        error = error }

let cancel model =
    { model with
        newVertexName = ""
        error = None
        page = Endpoint.MainMenu }

let view (model: Model) dispatch =
    Template
        .Main
        .AddVertexForm()
        .CancelButton(fun _ -> dispatch Msg.CancelAddVertex)
        .SaveButton(fun _ -> dispatch (Msg.AddVertex model.newVertexName))
        .VertexNameInput(model.newVertexName, (fun v -> dispatch (Msg.UpdateVertexName v)))
        .Elt()
