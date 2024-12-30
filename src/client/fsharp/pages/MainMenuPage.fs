module AAG.Client.MainMenuPage

open Model
open Elmish

let clearGraph<'a> : Model * Cmd<'a> = Model.Init, Cmd.none

let view (model: Model) dispatch =
    Template
        .Main
        .MainMenu()
        .OpenAddVertexFormButton(fun _ -> dispatch (Msg.SetPage Endpoint.AddVertex))
        .ClearGraphButton(fun _ -> dispatch (Msg.ClearGraph))
        .Elt()
