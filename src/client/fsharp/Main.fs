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
            |> Seq.collect (fun v -> AAG.getAccesses v.id model.graph)
        )

    let edgesLen = Seq.length (model.graph.edges)

    $"nodes: {nodeLen}; accesses; {accessLen} edges: {edgesLen}"

let private handleClickedBackground model dispatch =
    match model.step with
    | MainMenu -> dispatch Msg.IgnoreAction
    | ModifyVertex -> ModifyVertexStep.handleClickedBackground model dispatch
    | ModifyAccess -> ModifyAccessStep.handleClickedBackground model dispatch
    | AnalysisCompromise -> AnalysisCompromiseStep.handleClickedBackground dispatch

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
        | AnalysisCompromise -> AnalysisCompromiseStep.handleClickedVertex vertex model dispatch
    | None -> dispatch Msg.IgnoreAction

let private handleClickedVisEdge (idAsString: string) model dispatch =
    let access =
        match Guid.TryParse(idAsString) with
        | (true, guid) -> AAG.findAccessByEdgeId guid model.graph
        | (false, _) -> None

    match access with
    | Some access ->
        match model.step with
        | MainMenu -> MainMenuStep.handleClickedAccess access dispatch
        | ModifyVertex -> ModifyVertexStep.handleClickedEdge access model dispatch
        | _ -> dispatch Msg.IgnoreAction
    | None -> dispatch Msg.IgnoreAction

let getJsonOutputHint (model: Model) =
    match Json.tryDeserializeGraph model.json with
    | Graph _ -> Hint.Info
    | ErrorMsg msg -> msg


let handleUpdatedJson jsonAsString (model: Model) =
    match Json.tryDeserializeGraph jsonAsString with
    | Graph graph ->
        { Model.Init with
            graph = graph
            json = jsonAsString }
    | ErrorMsg _ -> { model with json = jsonAsString }

let private update message model =
    let model =
        match message with
        | Msg.IgnoreAction -> model
        // Main
        | Msg.SetPage page -> { model with page = page }
        | Msg.ModifiedGraphJson jsonAsString -> handleUpdatedJson jsonAsString model

        // MainMenu
        | Msg.OpenMainMenuStep -> MainMenuStep.``open`` model
        | Msg.ClickedClearGraph -> MainMenuStep.clearGraph
        | Msg.ClickedExampleGraph -> MainMenuStep.exampleGraph
        // ModifyAccess
        | Msg.OpenModifyAccessStep (access, addAccessNameInput) ->
            ModifyAccessStep.``open`` access addAccessNameInput model
        | Msg.ModifiedAccessName (access, name) -> ModifyAccessStep.updateAccessName access name model
        | Msg.ToggleFactorForSubject vertex -> ModifyAccessStep.toggleFactorForSubject vertex model
        | Msg.ClickedDeleteAccess access -> ModifyAccessStep.exitDeletingAccess access model
        // ModifyVertex
        | Msg.OpenModifyVertexStep vertexId -> ModifyVertexStep.``open`` vertexId model
        | Msg.ModifiedVertexName (subjectVertexId, name) -> ModifyVertexStep.updateVertexName subjectVertexId name model
        | Msg.ClickedDeleteVertex subjectVertex -> ModifyVertexStep.exitDeletingVertex subjectVertex model
        | Msg.HighlightAccess access -> highlightAccess access model
        | Msg.DeHighlightAccess -> deHighlightAccess model
        // AnalysisCompromise
        | Msg.OpenAnalysisCompromise -> AnalysisCompromiseStep.``open`` model
        | Msg.ToggleInitialCompromise vertex -> AnalysisCompromiseStep.toggleFactor vertex model

    model, Cmd.none

let private view (jsRuntime: IJSRuntime) model dispatch =
    jsRuntime.InvokeVoidAsync("updateNetwork", VisJSTransformer.transform model)
    |> ignore

    let model =
        if (getJsonOutputHint model).level <> Error then
            { model with json = Json.serializeGraph model.graph }
        else
            model

    Template
        .Main()
        .DebugText(getDebugText model) // TODO
        .JsonOutput(model.json, (fun jsonAsString -> dispatch (Msg.ModifiedGraphJson jsonAsString)))
        .JsonOutputHint((getJsonOutputHint model).value)
        .LeftColumn(
            cond model.page
            <| function
                | Endpoint.Main ->
                    match model.step with
                    | MainMenu -> MainMenuStep.view dispatch
                    | ModifyAccess -> ModifyAccessStep.view jsRuntime model dispatch
                    | ModifyVertex -> ModifyVertexStep.view jsRuntime model dispatch
                    | AnalysisCompromise -> AnalysisCompromiseStep.view jsRuntime model dispatch
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
