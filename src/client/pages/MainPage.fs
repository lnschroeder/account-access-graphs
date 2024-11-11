module AAG.Client.MainPage

open Bolero.Html

let private menuItem currentPage page (text: string) =
    Template
        .Main
        .MenuItem()
        .Active(
            if currentPage = page then
                "is-active"
            else
                ""
        )
        .Url(Router.router.Link page)
        .Text(text)
        .Elt()

let setPage (model: Model.Model) page = { model with page = page }

let setError (model: Model.Model) (exn: exn) = { model with error = Some exn.Message }

let clearError (model: Model.Model) = { model with error = None }

let view jsRuntime (model: Model.Model) dispatch =
    Template
        .Main()
        .Menu(concat { menuItem model.page Endpoint.Graph "Graph" })
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
