module AAG.Client.Routing

open Bolero

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

/// Connects the routing system to the Elmish application.
let router = Router.infer SetPage (fun (model: Model) -> model.page)
