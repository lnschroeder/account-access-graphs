module AAG.Client.AddAccessPage

open Elmish
open Model

let selectVertexForNewAccess name (model: Model) =
    if name = "" then
            { model with
                selectedVertexForNewAccess = "" }
    else
        let error =
            if not (AAG.isVertexWithNameInGraph name model.graph) then
                Some "vertex does not exist"
            else
                None
        { model with
            selectedVertexForNewAccess = name
            error = error }
let view (model: Model) dispatch =
    Template
        .Main
        .AddAccessForm()
        .CancelButton(fun _ -> dispatch (Msg.SetPage Endpoint.MainMenu))
        .VertexNameInput(model.selectedVertexForNewAccess, (fun v -> dispatch (Msg.SelectVertexForNewAccess v)))
        .Elt()
