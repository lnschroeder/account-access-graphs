module AAG.Client.MainPage

open Model
open Bolero.Html
open System

let setPage (model: Model) page = { model with page = page }

let private findVertexById idAsString graph =
    if idAsString = "undefined" then
        None
    else
        let guid = Guid.Parse idAsString
        AAG.findVertexById guid graph


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
                let vertex = findVertexById idAsString model.graph

                if model.page = Endpoint.AddAccess then
                    dispatch (Msg.ClickedVisNodeOnAddAccessPage vertex)
        )
        .Elt()
