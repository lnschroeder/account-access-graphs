module AAG.Client.Model

open System

/// The Elmish application's model.
type HintLevel =
    | Info
    | Warning
    | Error

type Hint =
    { value: string
      level: HintLevel }
    static member Info = { value = ""; level = Info }
    static member Error value = { value = value; level = Error }
    static member Required = Hint.Error "Field is required"

type Input =
    { value: string
      hint: Hint }
    static member private Required = { value = ""; hint = Hint.Required }
    static member private Optional = { value = ""; hint = Hint.Info }
    // Defaults
    static member AddVertexInput = Input.Required
    static member AddAccessInput = Input.Required
    static member SubjectInput = Input.Required

let isValidInput (input: Input) = input.hint.level <> Error

type Model =
    { page: Endpoint.Page
      step: string option
      graph: AAG.Graph
      addVertexInput: Input
      addAccessInput: Input
      subjectInput: Input
      subjectId: Guid option
      factorsInput: Guid Set }
    static member Init =
        { page = Endpoint.MainMenu
          step = None
          graph = AAG.Graph.Empty
          addVertexInput = Input.AddVertexInput
          addAccessInput = Input.AddAccessInput
          subjectInput = Input.SubjectInput
          subjectId = None
          factorsInput = Set.empty }

    static member Example = { Model.Init with graph = AAG.Graph.Example }
