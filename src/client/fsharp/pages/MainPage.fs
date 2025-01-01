module AAG.Client.MainPage

open Model
open Bolero.Html

let setPage (model: Model) page = { model with page = page }

let view jsRuntime (model: Model) dispatch =
    Template
        .Main()
        .LeftColumn(
            cond model.page
            <| function
                | Endpoint.MainMenu -> MainMenuPage.view dispatch
                | Endpoint.AddVertex -> AddVertexPage.view jsRuntime model dispatch
                | Endpoint.AddAccess -> AddAccessPage.view jsRuntime model dispatch
        )
        .ClickedNodeInput("", (
            fun name ->
                match model.page with
                    | Endpoint.AddAccess -> dispatch (Msg.SelectVertexForNewAccess name)
                    | _ -> ()
                )
            )
        .Elt()
