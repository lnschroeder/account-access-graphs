module AAG.Client.MainMenuPage

open Model

let clearGraph = Model.Init

let exampleGraph = Model.Example

let view dispatch =
    Template
        .MainMenu()
        .AddVertexButton(fun _ -> dispatch (Msg.SetPage Endpoint.AddVertex))
        .AddAccessButton(fun _ -> dispatch (Msg.SetPage Endpoint.AddAccess))
        .ClearGraphButton(fun _ -> dispatch (Msg.ClickedClearGraph))
        .ExampleGraphButton(fun _ -> dispatch (Msg.ClickedExampleGraph))
        .Elt()
