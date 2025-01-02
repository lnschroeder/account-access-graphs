module AAG.Client.Msg

/// The Elmish application's update messages.
type Message =
    | SetPage of Endpoint.Page
    | ClickedSaveAddVertexButton of string
    | TypedVertexName of string
    | ClickedVisNodeOnAddAccessPage of AAG.Vertex option
    | TypedSubjectName of string
    | ClickedContinueSubject
    | ClickedCancelAddAccessButton
    | ClickedCancelAddVertexButton
    | ClickedClearGraphButton
    | ClickedExampleGraphButton
