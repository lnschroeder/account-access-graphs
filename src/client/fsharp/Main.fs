module AAG.Client.Main

open System.Net.Http
open Microsoft.AspNetCore.Components
open Elmish
open Bolero
open Model

let private init _ = Model.Init, Cmd.none

let private update jsRuntime message model =
    match message with
    | Msg.SetPage page -> MainPage.setPage model page, Cmd.none
    | Msg.Error exn -> MainPage.setError model exn, Cmd.none
    | Msg.ClearError -> MainPage.clearError model, Cmd.none

    | Msg.AddVertex value -> AddVertexPage.addVertex value model jsRuntime
    | Msg.UpdateVertexName value -> AddVertexPage.updateVertexName value model

let private view jsRuntime model dispatch = MainPage.view jsRuntime model dispatch

type App() =
    inherit ProgramComponent<Model, Msg.Message>()

    override _.CssScope = CssScopes.AAG

    [<Inject>]
    member val HttpClient = Unchecked.defaultof<HttpClient> with get, set

    override this.Program =
        Program.mkProgram init (update this.JSRuntime) (view this.JSRuntime)
        |> Program.withRouter Router.router
