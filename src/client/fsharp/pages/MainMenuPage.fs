module AAG.Client.MainMenuPage

open Model
open Elmish

let clearGraph jsRuntime =
    Model.Init, Cmd.none

let view (model: Model) dispatch =
    Template
        .Main
        .MainMenu()
        .OpenAddVertexFormButton(fun _ -> dispatch (Msg.SetPage Endpoint.AddVertex))
        .ClearGraphButton(fun _ -> dispatch (Msg.ClearGraph))
        .Elt()
