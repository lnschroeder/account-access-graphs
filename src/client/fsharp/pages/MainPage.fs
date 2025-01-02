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
        .ClickedNodeInput(
            "",
            fun idAsString ->
                if model.page = Endpoint.AddAccess && model.step = None then
                    dispatch (Msg.SelectVertexForNewAccessById idAsString)
                elif model.page = Endpoint.AddAccess && model.step = Some "factors" then
                    dispatch (Msg.SelectFactorsForNewAccessById idAsString)
        )
        .Elt()
