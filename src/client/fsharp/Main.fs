module AAG.Client.Main

open System.Net.Http
open Microsoft.AspNetCore.Components
open Elmish
open Bolero
open Model
open Microsoft.JSInterop

let private invokeUpdateNetwork (graph: AAG.Graph) (jsRuntime: IJSRuntime) =
    let visNetwork = VisJSTransformer.transform graph

    jsRuntime.InvokeVoidAsync("updateNetwork", visNetwork.nodes, visNetwork.edges)
    |> ignore

let private init _ = MainMenuPage.clearGraph

let private update message model =
    let (model, cmd) =
        match message with
        | Msg.SetPage page -> MainPage.setPage model page
        | Msg.Error exn -> MainPage.setError model exn
        | Msg.ClearError -> MainPage.clearError model

        | Msg.AddVertex value -> AddVertexPage.addVertex value model
        | Msg.UpdateVertexName value -> AddVertexPage.updateVertexName value model

        | Msg.SelectVertexForNewAccess value -> AddAccessPage.selectVertexForNewAccess value model

        | Msg.ClearGraph -> MainMenuPage.clearGraph

    model, cmd

let private view jsRuntime model dispatch =
    invokeUpdateNetwork model.graph jsRuntime
    MainPage.view model dispatch

type App() =
    inherit ProgramComponent<Model, Msg.Message>()

    override _.CssScope = CssScopes.AAG

    [<Inject>]
    member val HttpClient = Unchecked.defaultof<HttpClient> with get, set

    override this.Program =
        Program.mkProgram init update (view this.JSRuntime)
        |> Program.withRouter Router.router
