module AAG.Client.MainMenuStep

open Model

let clearGraph = Model.Init

let exampleGraph = Model.Example

let openAddVertex model = { model with step = AddVertex }

let openAddAccess model = { model with step = AddAccessSubject }

let view jsRuntime (model: Model) dispatch =
    Template
        .MainMenu()
        .AddVertexButton(fun _ -> dispatch (Msg.ClickedAddVertex))
        .AddAccessButton(fun _ -> dispatch (Msg.ClickedAddAccess))
        .ClearGraphButton(fun _ -> dispatch (Msg.ClickedClearGraph))
        .ExampleGraphButton(fun _ -> dispatch (Msg.ClickedExampleGraph))
        .Elt()
