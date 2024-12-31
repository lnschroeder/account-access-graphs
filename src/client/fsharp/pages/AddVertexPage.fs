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
            newVertexName = ""
            newVertexNameValid = false }

let updateVertexName name (model: Model) =
    { model with
        newVertexName = name
        newVertexNameValid =
            not (
                AAG.isInvalidVertexName name
                || AAG.isVertexWithNameInGraph name model.graph
            ) }

let cancel model =
    { model with
        newVertexName = ""
        newVertexNameValid = false
        page = Endpoint.MainMenu }

let view (jsRuntime: IJSRuntime) (model: Model) dispatch =
    jsRuntime.InvokeVoidAsync("setButtonEnabled", "addVertexSaveButton", model.newVertexNameValid)
        |> ignore

    Template
        .Main
        .AddVertexForm()
        .CancelButton(fun _ -> dispatch Msg.CancelAddVertex)
        .SaveButton(fun _ -> dispatch (Msg.AddVertex model.newVertexName))
        .VertexNameInput(model.newVertexName, (fun v -> dispatch (Msg.UpdateVertexName v)))
        .Elt()
