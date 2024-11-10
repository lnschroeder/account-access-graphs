module AAG.Client.Main

open System.Net.Http
open Microsoft.AspNetCore.Components
open Elmish
open Bolero
open Bolero.Html
open Microsoft.JSInterop
open System

/// Routing endpoints definition.
type Page = | [<EndPoint "/">] Graph

/// The Elmish application's model.
type Model =
    { page: Page
      graph: AAG.Graph
      newNodeName: string
      error: string option }


/// The Elmish application's update messages.
type Message =
    | SetPage of Page
    | AddNode of string
    | UpdateNodeName of string
    | Error of exn
    | ClearError
    | CallJsFunction

let initModel =
    { page = Graph
      graph = { nodes = [||] }
      newNodeName = ""
      error = None }


let invokeUpdateNetwork (graph: AAG.Graph) (jsRuntime: IJSRuntime) =
    let visNetwork = Vis.graph2visNetwork graph

    jsRuntime.InvokeVoidAsync("updateNetwork", visNetwork.nodes, visNetwork.edges)
    |> ignore

let update (jsRuntime: IJSRuntime) message model =
    match message with
    | SetPage page -> { model with page = page }, Cmd.none

    | AddNode value when AAG.isInvalidNodeName model.graph value -> model, Cmd.none
    | AddNode value ->
        let newNode: AAG.Node =
            { id = Guid.NewGuid()
              name = value
              accesses = [||] }

        let newGraph: AAG.Graph = { nodes = Array.append model.graph.nodes [| newNode |] }

        invokeUpdateNetwork newGraph jsRuntime

        { model with
            graph = newGraph
            newNodeName = "" },
        Cmd.none

    | UpdateNodeName value ->
        let error =
            if AAG.isInvalidNodeName model.graph value then
                Some "invalid node name"
            else
                None

        { model with
            newNodeName = value
            error = error },
        Cmd.none

    | Error exn -> { model with error = Some exn.Message }, Cmd.none
    | ClearError -> { model with error = None }, Cmd.none

    | CallJsFunction ->
        // Use IJSRuntime to call the JavaScript function
        jsRuntime.InvokeVoidAsync("myJavaScriptFunction")
        |> ignore

        model, Cmd.none // Return the unchanged model

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

let init _ = initModel, Cmd.none

type MyApp() =
    inherit ProgramComponent<Model, Message>()

    override _.CssScope = CssScopes.AAG

    [<Inject>]
    member val HttpClient = Unchecked.defaultof<HttpClient> with get, set

    override this.Program =
        Program.mkProgram init (update this.JSRuntime) (view this.JSRuntime)
        |> Program.withRouter router
