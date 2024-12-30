module AAG.Client.MainMenuPage

open Microsoft.JSInterop
open Model

let private invokeUpdateNetwork (graph: AAG.Graph) (jsRuntime: IJSRuntime) =
    let visNetwork = VisJSTransformer.transform graph

    jsRuntime.InvokeVoidAsync("updateNetwork", visNetwork.nodes, visNetwork.edges)
    |> ignore

let view jsRuntime (model: Model) dispatch =
    invokeUpdateNetwork model.graph jsRuntime

    Template
        .Main
        .MainMenu()
        .OpenAddVertexFormButton(fun _ -> dispatch (Msg.SetPage Endpoint.AddVertex))
        .Elt()
