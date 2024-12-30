module AAG.Client.MainPage

open Model
open Bolero.Html
open Elmish

let setPage (model: Model) page = { model with page = page }, Cmd.none

let setError (model: Model) (exn: exn) =
    { model with error = Some exn.Message }, Cmd.none

let clearError (model: Model) = { model with error = None }, Cmd.none

let view (model: Model) dispatch =
    Template
        .Main()
        .LeftColumn(
            cond model.page
            <| function
                | Endpoint.MainMenu -> MainMenuPage.view model dispatch
                | Endpoint.AddVertex -> AddVertexPage.view model dispatch
        )
        .Error(
            cond model.error
            <| function
                | None -> empty ()
                | Some err ->
                    Template
                        .Main
                        .ErrorNotification()
                        .Text(err)
                        .Hide(fun _ -> dispatch Msg.ClearError)
                        .Elt()
        )
        .Elt()
