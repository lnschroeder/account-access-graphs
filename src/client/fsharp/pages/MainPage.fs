module AAG.Client.MainPage
open Model
open Bolero.Html

let setPage (model: Model) page = { model with page = page }

let setError (model: Model) (exn: exn) = { model with error = Some exn.Message }

let clearError (model: Model) = { model with error = None }

let view jsRuntime (model: Model) dispatch =
    Template
        .Main()
        .Body(
            cond model.page
            <| function
                | Endpoint.Graph -> GraphPage.view jsRuntime model dispatch
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
