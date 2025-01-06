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

let private handleClickedVisNode idAsString model dispatch =
    let vertex =
        if idAsString = "undefined" then
            None
        else
            AAG.findVertexById (Guid.Parse idAsString) model.graph

    match model.step with
    | MainMenu -> MainMenuStep.handleClickedVertex vertex model dispatch
    | ModifyVertex -> ModifyVertexStep.handleClickedVertex vertex model dispatch
    | ModifyAccess -> ModifyAccessStep.handleClickedVertex vertex model dispatch

let private update message model =
    let model =
        match message with
        | Msg.IgnoreAction -> model
        // Main
        | Msg.SetPage page -> { model with page = page }
        // MainMenuStep
        | Msg.OpenMainMenuStep -> MainMenuStep.``open`` model
        | Msg.ClickedClearGraph -> MainMenuStep.clearGraph
        | Msg.ClickedExampleGraph -> MainMenuStep.exampleGraph
        // ModifyAccessStep
        | Msg.OpenModifyAccessStep accessId -> ModifyAccessStep.``open`` accessId model
        | Msg.ModifiedAccessName (access, name) -> ModifyAccessStep.updateAccessName access name model
        | Msg.ToggleFactorOfSubjectAccess (accessId, vertex) -> ModifyAccessStep.toggleFactor accessId vertex model
        | Msg.ClickedDeleteAccess subjectAccessId -> ModifyAccessStep.exitDeletingAccess subjectAccessId model
        // ModifyVertex
        | Msg.OpenModifyVertexStep vertexId -> ModifyVertexStep.``open`` vertexId model
        | Msg.ModifiedVertexName (subjectVertexId, name) -> ModifyVertexStep.updateVertexName subjectVertexId name model

    model, Cmd.none

let private view (jsRuntime: IJSRuntime) model dispatch =
    jsRuntime.InvokeVoidAsync("updateNetwork", VisJSTransformer.transform model)
    |> ignore
    printfn "%A" model
    Template
        .Main()
        .LeftColumn(
            cond model.page
            <| function
                | Endpoint.Main ->
                    match model.step with
                    | MainMenu -> MainMenuStep.view dispatch
                    | ModifyAccess -> ModifyAccessStep.view jsRuntime model dispatch
                    | ModifyVertex -> ModifyVertexStep.view jsRuntime model dispatch
        )
        .ClickedNodeInput("", (fun idAsString -> handleClickedVisNode idAsString model dispatch))
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
