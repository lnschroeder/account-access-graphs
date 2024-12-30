module AAG.Client.Model

/// The Elmish application's model.
type Model =
    { page: Endpoint.Page
      graph: AAG.Graph
      newVertexName: string
      error: string option }
    static member Init =
        { page = Endpoint.Graph
          graph = AAG.Graph.Empty
          newVertexName = ""
          error = None }
