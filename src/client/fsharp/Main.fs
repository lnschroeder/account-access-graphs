module AAG.Client.Main

open System.Net.Http
open Microsoft.AspNetCore.Components
open Elmish
open Bolero
open Model
open Microsoft.JSInterop

let private init _ = MainMenuPage.clearGraph, Cmd.none

let private update message model =
    match message with
    // MainPage
    | Msg.SetPage page -> MainPage.setPage model page, Cmd.none
    | Msg.Error exn -> MainPage.setError model exn, Cmd.none
    | Msg.ClearError -> MainPage.clearError model, Cmd.none
    // MainMenuPage
    | Msg.ClearGraph -> MainMenuPage.clearGraph, Cmd.none
    // AddVertexPage
    | Msg.AddVertex value -> AddVertexPage.addVertex value model, Cmd.none
    | Msg.UpdateVertexName value -> AddVertexPage.updateVertexName value model, Cmd.none
    | Msg.CancelAddVertex -> AddVertexPage.cancel model, Cmd.none
    // AddAccessPage
    | Msg.SelectVertexForNewAccess value -> AddAccessPage.selectVertexForNewAccess value model, Cmd.none
    | Msg.CancelAddAccess -> AddAccessPage.cancel model, Cmd.none

let private invokeJS (jsRuntime: IJSRuntime) model =
    let visNetwork = VisJSTransformer.transform model

    jsRuntime.InvokeVoidAsync("updateNetwork", visNetwork.nodes, visNetwork.edges, visNetwork.nodeIdOfNewAccess)
    |> ignore

let private view jsRuntime model dispatch =
    invokeJS jsRuntime model
    MainPage.view jsRuntime model dispatch

type App() =
    inherit ProgramComponent<Model, Msg.Message>()

    override _.CssScope = CssScopes.AAG

    [<Inject>]
    member val HttpClient = Unchecked.defaultof<HttpClient> with get, set

    override this.Program =
        Program.mkProgram init update (view this.JSRuntime)
        |> Program.withRouter Router.router
