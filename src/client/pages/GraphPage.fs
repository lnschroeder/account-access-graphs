module AAG.Client.GraphPage

open Microsoft.JSInterop
open Microsoft.AspNetCore.Components
open Elmish


let invokeUpdateNetwork (graph: AAG.Graph) (jsRuntime: IJSRuntime) =
    let visNetwork = Vis.graph2visNetwork graph

    jsRuntime.InvokeVoidAsync("updateNetwork", visNetwork.nodes, visNetwork.edges)
    |> ignore

let addNode name (model: Routing.Model) jsRuntime =
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

let updateNodeName name (model: Routing.Model) =
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

let graphPage jsRuntime (model: Routing.Model) dispatch =
    invokeUpdateNetwork model.graph jsRuntime

    MainPage.Main
        .Graph()
        .AddNode(fun _ -> dispatch (Routing.AddNode model.newNodeName))
        .NodeName(model.newNodeName, (fun v -> dispatch (Routing.UpdateNodeName v)))
        .Elt()
