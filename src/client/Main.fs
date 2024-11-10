module AAG.Client.Main

open System.Net.Http
open Microsoft.AspNetCore.Components
open Elmish
open Bolero
open Bolero.Html
open Microsoft.JSInterop

/// Routing endpoints definition.
type Page = | [<EndPoint "/">] Graph

/// The Elmish application's model.
type Model =
    { page: Page
      graph: AAG.Graph
      newNodeName: string
      error: string option }
    static member Init =
        { page = Graph
          graph = AAG.Graph.Empty
          newNodeName = ""
          error = None }



/// The Elmish application's update messages.
type Message =
    | SetPage of Page
    | AddNode of string
    | UpdateNodeName of string
    | Error of exn
    | ClearError

let invokeUpdateNetwork (graph: AAG.Graph) (jsRuntime: IJSRuntime) =
    let visNetwork = Vis.graph2visNetwork graph

    jsRuntime.InvokeVoidAsync("updateNetwork", visNetwork.nodes, visNetwork.edges)
    |> ignore

let addNode name model jsRuntime =
    if AAG.isInvalidNodeName name then
        model, Cmd.none
    elif AAG.isNodeNameInGraph name model.graph then
        model, Cmd.none
    else
        let graph = AAG.addNode name model.graph

        invokeUpdateNetwork graph jsRuntime

        { model with
            graph = graph
            newNodeName = "" },
        Cmd.none

let updateNodeName name model =
    let error =
        if AAG.isInvalidNodeName name then
            Some "invalid node name"
        elif AAG.isNodeNameInGraph name model.graph then
            Some "node already exists"
        else
            None

    { model with
        newNodeName = name
        error = error },
    Cmd.none

let update (jsRuntime: IJSRuntime) message model =
    match message with
    | SetPage page -> { model with page = page }, Cmd.none
    | AddNode value -> addNode value model jsRuntime
    | UpdateNodeName value -> updateNodeName value model
    | Error exn -> { model with error = Some exn.Message }, Cmd.none
    | ClearError -> { model with error = None }, Cmd.none

/// Connects the routing system to the Elmish application.
let router = Router.infer SetPage (fun model -> model.page)

type Main = Template<"wwwroot/main.html">

let graphPage (jsRuntime: IJSRuntime) (model: Model) dispatch =
    invokeUpdateNetwork model.graph jsRuntime

    Main
        .Graph()
        .AddNode(fun _ -> dispatch (AddNode model.newNodeName))
        .NodeName(model.newNodeName, (fun v -> dispatch (UpdateNodeName v)))
        .Elt()

let menuItem (model: Model) (page: Page) (text: string) =
    Main
        .MenuItem()
        .Active(
            if model.page = page then
                "is-active"
            else
                ""
        )
        .Url(router.Link page)
        .Text(text)
        .Elt()

let view (jsRuntime: IJSRuntime) model dispatch =
    Main()
        .Menu(concat { menuItem model Graph "Graph" })
        .Body(
            cond model.page
            <| function
                | Graph -> graphPage jsRuntime model dispatch
        )
        .Error(
            cond model.error
            <| function
                | None -> empty ()
                | Some err ->
                    Main
                        .ErrorNotification()
                        .Text(err)
                        .Hide(fun _ -> dispatch ClearError)
                        .Elt()
        )
        .Elt()

let init _ = Model.Init, Cmd.none

type MyApp() =
    inherit ProgramComponent<Model, Message>()

    override _.CssScope = CssScopes.AAG

    [<Inject>]
    member val HttpClient = Unchecked.defaultof<HttpClient> with get, set

    override this.Program =
        Program.mkProgram init (update this.JSRuntime) (view this.JSRuntime)
        |> Program.withRouter router
