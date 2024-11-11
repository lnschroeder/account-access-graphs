module AAG.Client.Router

open Bolero

/// Connects the routing system to the Elmish application.
let router = Router.infer Msg.SetPage (fun (model: ElmishModel.Model) -> model.page)
