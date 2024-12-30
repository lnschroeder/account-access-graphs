module AAG.Client.MainMenuPage

open Microsoft.JSInterop
open Model
open Elmish

let private invokeUpdateNetwork (graph: AAG.Graph) (jsRuntime: IJSRuntime) =
    let visNetwork = VisJSTransformer.transform graph

    jsRuntime.InvokeVoidAsync("updateNetwork", visNetwork.nodes, visNetwork.edges)
    |> ignore

let clearGraph jsRuntime =
    Model.Init, Cmd.none

let view jsRuntime (model: Model) dispatch =
    invokeUpdateNetwork model.graph jsRuntime

    Template
        .Main
        .MainMenu()
        .OpenAddVertexFormButton(fun _ -> dispatch (Msg.SetPage Endpoint.AddVertex))
        .ClearGraphButton(fun _ -> dispatch (Msg.ClearGraph))
        .Elt()
