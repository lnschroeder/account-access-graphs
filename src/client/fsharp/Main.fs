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

let private getDebugText (model: Model) =
    let nodeLen = Seq.length model.graph.vertices

    let accessLen =
        Seq.length (
            model.graph.vertices
            |> Seq.collect (fun v -> v.accesses)
        )

    let edgesLen =
        Seq.length (
            model.graph.vertices
            |> Seq.collect (fun v -> v.accesses |> Seq.collect (fun a -> a.factors))
        )

    $"nodes: {nodeLen}; accesses; {accessLen} edges: {edgesLen}"

let private handleClickedBackground model dispatch =
    match model.step with
    | MainMenu -> dispatch Msg.IgnoreAction
    | ModifyVertex -> ModifyVertexStep.handleClickedBackground model dispatch
    | ModifyAccess -> dispatch Msg.IgnoreAction

let private handleClickedVertex (idAsString: string) model dispatch =
    let vertex =
        match Guid.TryParse(idAsString) with
        | (true, guid) -> AAG.findVertexById guid model.graph
        | (false, _) -> None

    match vertex with
    | Some vertex ->
        match model.step with
        | MainMenu -> MainMenuStep.handleClickedVertex vertex model dispatch
        | ModifyVertex -> ModifyVertexStep.handleClickedVertex vertex model dispatch
        | ModifyAccess -> ModifyAccessStep.handleClickedVertex vertex model dispatch
    | None -> dispatch Msg.IgnoreAction

let private handleClickedVisEdge (idAsString: string) model dispatch =
    let vertexIdAccess =
        match Guid.TryParse(idAsString) with
        | (true, guid) -> AAG.findAccessByEdgeId guid model.graph
        | (false, _) -> None

    match vertexIdAccess with
    | Some (vertexId, access) ->
        match model.step with
        | MainMenu -> MainMenuStep.handleClickedAccess access model dispatch
        | ModifyVertex -> ModifyVertexStep.handleClickedAccess vertexId access model dispatch
        | ModifyAccess -> dispatch Msg.IgnoreAction
    | None -> dispatch Msg.IgnoreAction

let private update message model =
    let model =
        match message with
        | Msg.IgnoreAction -> model
        // Main
        | Msg.SetPage page -> { model with page = page }
        | Msg.ModifiedGraphJson graphAsString -> { Model.Init with graph = (Json.deserializeGraph graphAsString) }
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
        | Msg.ClickedDeleteVertex subjectVertex -> ModifyVertexStep.exitDeletingVertex subjectVertex model

    model, Cmd.none

let private view (jsRuntime: IJSRuntime) model dispatch =
    jsRuntime.InvokeVoidAsync("updateNetwork", VisJSTransformer.transform model)
    |> ignore

    Template
        .Main()
        .DebugText(getDebugText model)
        .JsonOutput(Json.serializeGraph model.graph, (fun graphString -> dispatch (Msg.ModifiedGraphJson graphString)))
        .LeftColumn(
            cond model.page
            <| function
                | Endpoint.Main ->
                    match model.step with
                    | MainMenu -> MainMenuStep.view dispatch
                    | ModifyAccess -> ModifyAccessStep.view jsRuntime model dispatch
                    | ModifyVertex -> ModifyVertexStep.view jsRuntime model dispatch
        )
        .ClickedVertexInput("", (fun idAsString -> handleClickedVertex idAsString model dispatch))
        .ClickedEdgeInput("", (fun idAsString -> handleClickedVisEdge idAsString model dispatch))
        .ClickedBackgroundButton(fun _ -> handleClickedBackground model dispatch)
        .Elt()


let private router = Router.infer Msg.SetPage (fun (model: Model) -> model.page)

type App() =
    inherit ProgramComponent<Model, Msg.Message>()

    [<Inject>]
    member val HttpClient = Unchecked.defaultof<HttpClient> with get, set

    override this.Program =
        Program.mkProgram init update (view this.JSRuntime)
        |> Program.withRouter router
