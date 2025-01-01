module AAG.Client.AddAccessPage

open Model
open Microsoft.JSInterop

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


let view (jsRuntime: IJSRuntime) (model: Model) dispatch =
    jsRuntime.InvokeVoidAsync(
        "setButtonDisabled",
        "selectVertexForNewAccessButton",
        isInvalidInput model.vertexForNewAccessInput
    )
    |> ignore

    Template
        .AddAccessForm()
        .CancelButton(fun _ -> dispatch Msg.CancelAddAccess)
        .VertexNameInput(model.vertexForNewAccessInput.value, (fun v -> dispatch (Msg.SelectVertexForNewAccess v)))
        .SelectVertexHint(model.vertexForNewAccessInput.hint.value)
        .Elt()
