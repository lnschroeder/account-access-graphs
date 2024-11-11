module AAG.Client.Model

/// The Elmish application's model.
type Model =
    { page: Endpoint.Page
      graph: AAG.Graph
      newNodeName: string
      error: string option }
    static member Init =
        { page = Endpoint.Graph
          graph = AAG.Graph.Empty
          newNodeName = ""
          error = None }
