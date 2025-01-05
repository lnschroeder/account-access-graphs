module AAG.Client.Main

open System.Net.Http
open Microsoft.AspNetCore.Components
open Elmish
open Bolero
open Model
open Microsoft.JSInterop
open System
open Bolero.Html

let private init _ = MainMenuStep.clearGraph, Cmd.none

let private handleClickedVisNode idAsString model =
    let vertex =
        if idAsString = "undefined" then
            None
        else
            let guid = Guid.Parse idAsString
            AAG.findVertexById guid model.graph

    match model.step with
    | ModifyAccess -> ModifyAccessStep.handleClickedVertex vertex model
    | MainMenu -> MainMenuStep.handleClickedVertex vertex model
    | ModifyVertex -> MainMenuStep.handleClickedVertex vertex model
    | _ -> model

let private update message model =
    let model =
        match message with
        // Main
        | Msg.SetPage page -> { model with page = page }
        | Msg.ClickedVisNode idAsString -> handleClickedVisNode idAsString model
        // MainMenuStep
        | Msg.ClickedClearGraph -> MainMenuStep.clearGraph
        | Msg.ClickedExampleGraph -> MainMenuStep.exampleGraph
        | Msg.ClickedAddVertex -> MainMenuStep.openAddVertex model
        // AddVertexStep
        | Msg.ClickedSaveAddVertex -> AddVertexStep.addNewVertex model
        | Msg.TypedVertexName value -> AddVertexStep.updateVertexName value model
        | Msg.ClickedCancelAddVertex -> AddVertexStep.cancel model
        // AddAccessStep
        | Msg.TypedAccessName name -> ModifyAccessStep.updateAccessName name model
        | Msg.ClickedBackFromSubjectAccess -> ModifyAccessStep.saveSubjectAccess model
        | Msg.ClickedDeleteAccess -> ModifyAccessStep.exitDeletingProvisionalAccesses model
        | Msg.ClickedRemoveProvisionalFactor id -> ModifyAccessStep.removeProvisionalFactor id model
        // ModifyVertex
        | Msg.ModifiedVertexName (subjectVertexId, name) -> ModifyVertexStep.updateVertexName subjectVertexId name model
        | Msg.ClickedModifyAccess -> ModifyVertexStep.openModifyAccess model
        | Msg.ClickedAddAccess -> ModifyVertexStep.openModifyAccess model
        | Msg.ClickedBackFromSubjectVertex -> ModifyVertexStep.saveAndExit model

    model, Cmd.none

let private view (jsRuntime: IJSRuntime) model dispatch =
    jsRuntime.InvokeVoidAsync("updateNetwork", VisJSTransformer.transform model)
    |> ignore

    Template
        .Main()
        .LeftColumn(
            cond model.page
            <| function
                | Endpoint.Main ->
                    match model.step with
                    | MainMenu -> MainMenuStep.view dispatch
                    | AddVertex -> AddVertexStep.view jsRuntime model dispatch
                    | ModifyAccess -> ModifyAccessStep.view jsRuntime model dispatch
                    | ModifyVertex -> ModifyVertexStep.view jsRuntime model dispatch
        )
        .ClickedNodeInput("", (fun idAsString -> dispatch (Msg.ClickedVisNode idAsString)))
        .Elt()


let private router = Router.infer Msg.SetPage (fun (model: Model) -> model.page)

type App() =
    inherit ProgramComponent<Model, Msg.Message>()

    override _.CssScope = CssScopes.AAG

    [<Inject>]
    member val HttpClient = Unchecked.defaultof<HttpClient> with get, set

    override this.Program =
        Program.mkProgram init update (view this.JSRuntime)
        |> Program.withRouter router
