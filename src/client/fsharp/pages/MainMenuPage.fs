module AAG.Client.MainMenuPage

open Model

let clearGraph = Model.Init

let view dispatch =
    Template
        .Main
        .MainMenu()
        .OpenAddVertexFormButton(fun _ -> dispatch (Msg.SetPage Endpoint.AddVertex))
        .OpenAddAccessFormButton(fun _ -> dispatch (Msg.SetPage Endpoint.AddAccess))
        .ClearGraphButton(fun _ -> dispatch (Msg.ClearGraph))
        .Elt()
