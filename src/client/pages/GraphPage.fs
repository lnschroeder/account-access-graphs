module AAG.Client.GraphPage

open Microsoft.JSInterop
open Elmish

let private invokeUpdateNetwork (graph: AAG.Graph) (jsRuntime: IJSRuntime) =
    let visNetwork = VisJSTransformer.transform graph

    jsRuntime.InvokeVoidAsync("updateNetwork", visNetwork.nodes, visNetwork.edges)
    |> ignore

let addNode name (model: ElmishModel.Model) jsRuntime =
    if AAG.isInvalidNodeName name then
        model, Cmd.none
    elif AAG.isNodeNameInGraph name model.graph then
        model, Cmd.none
    else
        let graph = AAG.addNode name model.graph

        invokeUpdateNetwork graph jsRuntime

        { model with
            graph = graph
            newNodeName = "" },
        Cmd.none

let updateNodeName name (model: ElmishModel.Model) =
    let error =
        if AAG.isInvalidNodeName name then
            Some "invalid node name"
        elif AAG.isNodeNameInGraph name model.graph then
            Some "node already exists"
        else
            None

    { model with
        newNodeName = name
        error = error },
    Cmd.none

let view jsRuntime (model: ElmishModel.Model) dispatch =
    invokeUpdateNetwork model.graph jsRuntime

    Template
        .Main
        .Graph()
        .AddNode(fun _ -> dispatch (Msg.AddNode model.newNodeName))
        .NodeName(model.newNodeName, (fun v -> dispatch (Msg.UpdateNodeName v)))
        .Elt()
