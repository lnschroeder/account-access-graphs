module AAG.Client.Model

/// The Elmish application's model.
type HintType =
    | Info
    | Warning
    | Error

type Hint =
    { value: string
      ``type``: HintType }

    static member Info = { value = ""; ``type`` = Info }

    static member Error value = { value = value; ``type`` = Error }

    static member Required = Hint.Error "Field is required"

type Input =
    { value: string
      hint: Hint }
    static member Required = { value = ""; hint = Hint.Required }

    static member Optional = { value = ""; hint = Hint.Info }

let isInvalidInput (input: Input) = input.hint.``type`` = Error

type Model =
    { page: Endpoint.Page
      graph: AAG.Graph
      newVertexNameInput: Input
      selectedVertexForNewAccess: string
      error: string option }
    static member Init =
        { page = Endpoint.MainMenu
          graph = AAG.Graph.Empty
          newVertexNameInput = Input.Required
          selectedVertexForNewAccess = ""
          error = None }
