module AAG.Client.Router

open Bolero
open Model

/// Connects the routing system to the Elmish application.
let router = Router.infer Msg.SetPage (fun (model: Model) -> model.page)
