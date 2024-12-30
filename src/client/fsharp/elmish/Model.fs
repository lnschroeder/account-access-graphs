module AAG.Client.Model

/// The Elmish application's model.
type Model =
    { page: Endpoint.Page
      graph: AAG.Graph
      newVertexName: string
      selectedVertexForNewAccess: string
      error: string option }
    static member Init =
        { page = Endpoint.MainMenu
          graph = AAG.Graph.Empty
          newVertexName = ""
          selectedVertexForNewAccess = ""
          error = None }
