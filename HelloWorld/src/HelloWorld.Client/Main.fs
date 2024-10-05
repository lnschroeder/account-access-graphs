module HelloWorld.Client.Main

open System.Net.Http
open Microsoft.AspNetCore.Components
open Elmish
open Bolero
open Bolero.Html

/// Routing endpoints definition.
type Page =
    | [<EndPoint "/">] Home
    | [<EndPoint "/graph">] Graph

/// The Elmish application's model.
type Model =
    { page: Page
      graph: Graph
      newNodeName: string
      error: string option }

and Node = { name: string; accesses: Access[] }

and Access = { access: string[] }

and Graph = { nodes: Node[] }

let initModel =
    { page = Home
      graph = { nodes = [||] }
      newNodeName = ""
      error = None }


/// The Elmish application's update messages.
type Message =
    | SetPage of Page
    | AddNode of string
    | UpdateNodeName of string
    | Error of exn
    | ClearError

let update message model =
    match message with
    | SetPage page -> { model with page = page }, Cmd.none

    | AddNode "" -> model, Cmd.none
    | AddNode value ->
        let newNode = { name = value; accesses = [||] }

        { model with
            graph = { nodes = Array.append model.graph.nodes [| newNode |] } },
        Cmd.none

    | UpdateNodeName value ->
        let error = if value = "" then Some "invalid node name" else None

        { model with
            newNodeName = value
            error = error },
        Cmd.none

    | Error exn -> { model with error = Some exn.Message }, Cmd.none
    | ClearError -> { model with error = None }, Cmd.none

/// Connects the routing system to the Elmish application.
let router = Router.infer SetPage (fun model -> model.page)

type Main = Template<"wwwroot/main.html">

let homePage model dispatch = Main.Home().Elt()

let graphPage (model: Model) dispatch =
    Main
        .Graph()
        .AddNode(fun _ -> dispatch (AddNode model.newNodeName))
        .NodeName("model.newNodeName", fun v -> dispatch (UpdateNodeName v))
        .NodeNames(model.graph.nodes |> Seq.map (fun node -> node.name) |> String.concat "; ")
        .Elt()

let menuItem (model: Model) (page: Page) (text: string) =
    Main
        .MenuItem()
        .Active(if model.page = page then "is-active" else "")
        .Url(router.Link page)
        .Text(text)
        .Elt()

let view model dispatch =
    Main()
        .Menu(
            concat {
                menuItem model Home "Home"
                menuItem model Graph "Graph"
            }
        )
        .Body(
            cond model.page
            <| function
                | Home -> homePage model dispatch
                | Graph -> graphPage model dispatch
        )
        .Error(
            cond model.error
            <| function
                | None -> empty ()
                | Some err -> Main.ErrorNotification().Text(err).Hide(fun _ -> dispatch ClearError).Elt()
        )
        .Elt()

let init _ = initModel, Cmd.none


type MyApp() =
    inherit ProgramComponent<Model, Message>()

    override _.CssScope = CssScopes.HelloWorld

    [<Inject>]
    member val HttpClient = Unchecked.defaultof<HttpClient> with get, set

    override this.Program = Program.mkProgram init update view |> Program.withRouter router
