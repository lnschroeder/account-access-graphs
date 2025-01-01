module AAG.Client.AddVertexPage

open Model
open Microsoft.JSInterop

let addVertex name (model: Model) =
    if AAG.isInvalidVertexName name then
        model
    elif AAG.isVertexWithNameInGraph name model.graph then
        model
    else
        { model with
            graph = AAG.addVertex name model.graph
            newVertexNameInput = Model.Init.newVertexNameInput }

let updateVertexName name (model: Model) =
    let hint =
        if name = "" then
            Input.NewVertexNameInput.hint
        elif AAG.isInvalidVertexName name then
            Hint.Error "Invalid name"
        elif AAG.isVertexWithNameInGraph name model.graph then
            Hint.Error "Vertex already exists"
        else
            Hint.Info

    { model with
        newVertexNameInput =
            { model.newVertexNameInput with
                value = name
                hint = hint } }

let cancel model =
    { model with
        newVertexNameInput = Input.NewVertexNameInput
        page = Endpoint.MainMenu }

let view (jsRuntime: IJSRuntime) (model: Model) dispatch =
    jsRuntime.InvokeVoidAsync("setButtonDisabled", "SaveButton", isInvalidInput model.newVertexNameInput)
    |> ignore

    Template
        .AddVertexForm()
        .CancelButton(fun _ -> dispatch Msg.CancelAddVertex)
        .SaveButton(fun _ -> dispatch (Msg.AddVertex model.newVertexNameInput.value))
        .VertexNameInput(model.newVertexNameInput.value, (fun v -> dispatch (Msg.UpdateVertexName v)))
        .VertexNameHint(model.newVertexNameInput.hint.value)
        .Elt()
