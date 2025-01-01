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
    static member private Required = { value = ""; hint = Hint.Required }
    static member private Optional = { value = ""; hint = Hint.Info }
    // Defaults
    static member NewVertexNameInput = Input.Required
    static member VertexForNewAccessInput = Input.Required


let isInvalidInput (input: Input) = input.hint.``type`` = Error

type Model =
    { page: Endpoint.Page
      graph: AAG.Graph
      newVertexNameInput: Input
      vertexForNewAccessInput: Input }
    static member Init =
        { page = Endpoint.MainMenu
          graph = AAG.Graph.Empty
          newVertexNameInput = Input.NewVertexNameInput
          vertexForNewAccessInput = Input.VertexForNewAccessInput }
    static member Example =
        { Model.Init with graph = AAG.Graph.Example }
