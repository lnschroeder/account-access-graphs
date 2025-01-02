module AAG.Client.Main

open System.Net.Http
open Microsoft.AspNetCore.Components
open Elmish
open Bolero
open Model
open Microsoft.JSInterop

let private init _ = MainMenuPage.clearGraph, Cmd.none

let private update message model =
    let model =
        match message with
        // MainPage
        | Msg.SetPage page -> MainPage.setPage model page
        // MainMenuPage
        | Msg.ClearGraph -> MainMenuPage.clearGraph
        | Msg.ExampleGraph -> MainMenuPage.exampleGraph
        // AddVertexPage
        | Msg.AddVertex value -> AddVertexPage.addVertex value model
        | Msg.UpdateVertexName value -> AddVertexPage.updateVertexName value model
        | Msg.CancelAddVertex -> AddVertexPage.cancel model
        // AddAccessPage
        | Msg.SelectVertexForNewAccess value -> AddAccessPage.selectVertexForNewAccess value model
        | Msg.CancelAddAccess -> AddAccessPage.cancel model
        | Msg.ContinueAddAccessSubject -> AddAccessPage.continueSubject model

    model, Cmd.none

let private view (jsRuntime: IJSRuntime) model dispatch =
    jsRuntime.InvokeVoidAsync("updateNetwork", VisJSTransformer.transform model)
    |> ignore

    MainPage.view jsRuntime model dispatch

let private router = Router.infer Msg.SetPage (fun (model: Model) -> model.page)

type App() =
    inherit ProgramComponent<Model, Msg.Message>()

    override _.CssScope = CssScopes.AAG

    [<Inject>]
    member val HttpClient = Unchecked.defaultof<HttpClient> with get, set

    override this.Program =
        Program.mkProgram init update (view this.JSRuntime)
        |> Program.withRouter router
