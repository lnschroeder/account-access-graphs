module AAG.Client.MainPage
open Model
open Bolero.Html
open Elmish
open Microsoft.JSInterop

let private invokeUpdateNetwork (graph: AAG.Graph) (jsRuntime: IJSRuntime) =
    let visNetwork = VisJSTransformer.transform graph

    jsRuntime.InvokeVoidAsync("updateNetwork", visNetwork.nodes, visNetwork.edges)
    |> ignore


let setPage (model: Model) page = { model with page = page }, Cmd.none

let setError (model: Model) (exn: exn) = { model with error = Some exn.Message }, Cmd.none

let clearError (model: Model) = { model with error = None }, Cmd.none

let view jsRuntime (model: Model) dispatch =
    invokeUpdateNetwork model.graph jsRuntime

    Template
        .Main()
        .LeftColumn(
            cond model.page
            <| function
                | Endpoint.MainMenu -> MainMenuPage.view model dispatch
                | Endpoint.AddVertex -> AddVertexPage.view model dispatch
        )
        .Error(
            cond model.error
            <| function
                | None -> empty ()
                | Some err ->
                    Template
                        .Main
                        .ErrorNotification()
                        .Text(err)
                        .Hide(fun _ -> dispatch Msg.ClearError)
                        .Elt()
        )
        .Elt()
