module AAG.Client.Main

open System.Net.Http
open Microsoft.AspNetCore.Components
open Elmish
open Bolero
open Model

let private init jsRuntime _ = MainMenuPage.clearGraph jsRuntime

let private update jsRuntime message model =
    match message with
    | Msg.SetPage page-> MainPage.setPage model page
    | Msg.Error exn -> MainPage.setError model exn
    | Msg.ClearError -> MainPage.clearError model

    | Msg.AddVertex value -> AddVertexPage.addVertex value model jsRuntime
    | Msg.UpdateVertexName value -> AddVertexPage.updateVertexName value model

    | Msg.ClearGraph -> MainMenuPage.clearGraph jsRuntime

let private view jsRuntime model dispatch = MainPage.view jsRuntime model dispatch

type App() =
    inherit ProgramComponent<Model, Msg.Message>()

    override _.CssScope = CssScopes.AAG

    [<Inject>]
    member val HttpClient = Unchecked.defaultof<HttpClient> with get, set

    override this.Program =
        Program.mkProgram (init this.JSRuntime) (update this.JSRuntime) (view this.JSRuntime)
        |> Program.withRouter Router.router
