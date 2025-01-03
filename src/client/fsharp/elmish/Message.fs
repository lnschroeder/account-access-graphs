module AAG.Client.Msg

/// The Elmish application's update messages.
type Message =
    | SetPage of Endpoint.Page
    | ClickedSaveAddVertexButton // TODO remove Clicked Messages
    | TypedVertexName of string
    | TypedAccessName of string
    | ClickedVisNodeOnAddAccessPage of AAG.Vertex option
    | TypedSubjectName of string
    | ClickedContinueSubject
    | ClickedCancelAddAccessButton
    | ClickedBackFromFactors
    | ClickedSaveAddAccessButton
    | ClickedCancelAddVertexButton
    | ClickedClearGraphButton
    | ClickedExampleGraphButton
