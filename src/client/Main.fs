module AAG.Client.Main

open System.Net.Http
open Microsoft.AspNetCore.Components
open Elmish
open Bolero
open Bolero.Html

let init _ = Routing.Model.Init, Cmd.none

let update jsRuntime message (model: Routing.Model) =
    match message with
    | Routing.SetPage page -> { model with page = page }, Cmd.none
    | Routing.AddNode value -> GraphPage.addNode value model jsRuntime
    | Routing.UpdateNodeName value -> GraphPage.updateNodeName value model
    | Routing.Error exn -> { model with error = Some exn.Message }, Cmd.none
    | Routing.ClearError -> { model with error = None }, Cmd.none

let view jsRuntime (model: Routing.Model) dispatch =
    MainPage.Main()
        .Menu(concat { MainPage.menuItem model.page Routing.Graph "Graph" })
        .Body(
            cond model.page
            <| function
                | Routing.Graph -> GraphPage.graphPage jsRuntime model dispatch
        )
        .Error(
            cond model.error
            <| function
                | None -> empty ()
                | Some err ->
                    MainPage.Main
                        .ErrorNotification()
                        .Text(err)
                        .Hide(fun _ -> dispatch Routing.ClearError)
                        .Elt()
        )
        .Elt()

type MyApp() =
    inherit ProgramComponent<Routing.Model, Routing.Message>()

    override _.CssScope = CssScopes.AAG

    [<Inject>]
    member val HttpClient = Unchecked.defaultof<HttpClient> with get, set

    override this.Program =
        Program.mkProgram init (update this.JSRuntime) (view this.JSRuntime)
        |> Program.withRouter Routing.router
