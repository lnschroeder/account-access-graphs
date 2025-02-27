module AAG.Client.Main

open System.Net.Http
open System.Net.Http.Json
open Microsoft.AspNetCore.Components
open Elmish
open Bolero
open Model
open Microsoft.JSInterop
open System
open Bolero.Html

/// The main `init` function of the Elmish program.
/// Initializes the model and command for the Elmish program.
/// This function is called once at the start of the program to set up the initial state.
let private init _ = MainMenuView.clearGraph, Cmd.none

let private getDebugText (model: Model) =
    let nodeLen = Seq.length model.graph.vertices

    let accessLen =
        Seq.length (
            model.graph.vertices
            |> Seq.collect (fun v -> AAG.getEnabledAccesses v.id model.graph)
        )

    let edgesLen = Seq.length model.graph.edges
    let componentsLen = Seq.length model.graph.components

    $"nodes: {nodeLen}; accesses: {accessLen}; edges: {edgesLen}; components: {componentsLen}"

let private handleClickedBackground model dispatch =
    match model.view with
    | MainMenu -> MainMenuView.handleClickedBackground dispatch
    | ModifyVertex -> ModifyVertexView.handleClickedBackground model dispatch
    | ModifyAccess -> ModifyAccessView.handleClickedBackground model dispatch
    | ModifyComponent -> ModifyComponentView.handleClickedBackground model dispatch
    | AnalysisManual -> AnalysisManualView.handleClickedBackground model dispatch
    | AnalysisAutomated -> AnalysisAutomatedView.handleClickedBackground model dispatch
    | Vinit -> VinitView.handleClickedBackground dispatch
    | AddComponent -> dispatch Msg.OpenMainMenuView

let private handleClickedVertex (idAsString: string) model dispatch =
    let vertex =
        match Guid.TryParse idAsString with
        | true, guid -> AAG.tryFindVertexById guid model.graph
        | false, _ -> None

    match vertex with
    | Some vertex ->
        match model.view with
        | MainMenu
        | AddComponent -> MainMenuView.handleClickedVertex vertex dispatch
        | ModifyVertex -> ModifyVertexView.handleClickedVertex vertex model dispatch
        | ModifyComponent -> dispatch Msg.IgnoreAction
        | ModifyAccess -> ModifyAccessView.handleClickedVertex vertex dispatch
        | AnalysisManual -> AnalysisManualView.handleClickedVertex model vertex dispatch
        | AnalysisAutomated -> AnalysisAutomatedView.handleClickedVertex vertex dispatch
        | Vinit -> VinitView.handleClickedVertex vertex dispatch
    | None -> dispatch Msg.IgnoreAction

let private handleClickedVisEdge (idAsString: string) model dispatch =
    let access =
        match Guid.TryParse idAsString with
        | true, guid -> AAG.tryFindAccessByEdgeId guid model.graph
        | false, _ -> None

    match access with
    | Some access ->
        match model.view with
        | MainMenu
        | AddComponent -> MainMenuView.handleClickedAccess access dispatch
        | _ -> dispatch Msg.IgnoreAction
    | None -> dispatch Msg.IgnoreAction

let private getJsonOutputHint (model: Model) =
    match Json.tryDeserializeGraph model.json with
    | Graph _ -> Hint.Info
    | ErrorMsg msg -> msg

let private handleUpdatedJson jsonAsString (model: Model) =
    match Json.tryDeserializeGraph jsonAsString with
    | Graph graph ->
        { Model.Init with
            graph = graph
            physics = model.physics
            edgeLabels = model.edgeLabels
            json = jsonAsString }
    | ErrorMsg _ -> { model with json = jsonAsString }

/// The main `update` function of the Elmish program.
/// Updates the model and command i.e. the state of the program.
let private update (http: HttpClient) message model =
    match message with
    | Msg.IgnoreAction -> model, Cmd.none
    | Msg.Error _ -> model, Cmd.none
    // Main
    | Msg.SetPage page -> { model with page = page }, Cmd.none
    | Msg.ModifiedGraphJson jsonAsString -> handleUpdatedJson jsonAsString model, Cmd.none
    | Msg.GotGraph graph -> { Model.Init with graph = graph }, Cmd.none
    | Msg.SetPhysics b -> { model with physics = b }, Cmd.none
    | Msg.SetEdgeLabels b -> { model with edgeLabels = b }, Cmd.none
    // MainMenu
    | Msg.OpenMainMenuView -> MainMenuView.``open`` model, Cmd.none
    | Msg.ClickedClearGraph -> MainMenuView.clearGraph, Cmd.none
    | Msg.ClickedExampleGraph ->
        let getGraph () =
            http.GetFromJsonAsync<AAG.Graph> "resources/aag/example.json"

        let cmd = Cmd.OfTask.either getGraph () Msg.GotGraph Msg.Error
        model, cmd
    // ModifyAccess
    | Msg.OpenModifyAccessView (access, addAccessNameInput) ->
        if AAG.getEdgesForAccess model.graph access
           |> List.filter (fun e -> e.component_.IsSome)
           |> List.isEmpty then
            ModifyAccessView.``open`` access addAccessNameInput model, Cmd.none
        else
            match AAG.tryFindVertexById access.vertexId model.graph with
            | Some vertex when vertex.component_.IsSome ->
                ModifyComponentView.``open``
                    { model with subjectVertexId = Some vertex.id }
                    (Option.get vertex.component_),
                Cmd.none
            | _ -> ModifyAccessView.``open`` access addAccessNameInput model, Cmd.none
    | Msg.ModifiedAccessName (access, name) -> ModifyAccessView.updateAccessName access name model, Cmd.none
    | Msg.ToggleFactorForSubject vertex -> ModifyAccessView.toggleFactorForSubject vertex model, Cmd.none
    | Msg.ClickedDeleteAccess access -> ModifyAccessView.exitDeletingAccess access model, Cmd.none
    // ModifyVertex
    | Msg.OpenModifyVertexView vertexId -> ModifyVertexView.``open`` vertexId model, Cmd.none
    | Msg.ModifiedVertexName (subjectVertexId, name) ->
        ModifyVertexView.updateVertexName subjectVertexId name model, Cmd.none
    | Msg.ClickedDeleteVertex subjectVertex -> ModifyVertexView.exitDeletingVertex subjectVertex model, Cmd.none
    | Msg.HighlightAccess access -> highlightAccess access model, Cmd.none
    | Msg.DeHighlightAccess -> deHighlightAccess model, Cmd.none
    | Msg.SetScore (vertex, score) -> ModifyVertexView.setScore vertex score model, Cmd.none
    // AnalysisManual
    | Msg.OpenAnalysisManual -> AnalysisManualView.``open`` model, Cmd.none
    // AnalysisAutomated
    | Msg.OpenAnalysis -> AnalysisAutomatedView.``open`` model, Cmd.none
    | Msg.OpenAnalysisForVertex vertexId -> AnalysisAutomatedView.showAnalysisForSubject model vertexId, Cmd.none
    // AddComponent
    | Msg.OpenAddComponentView ->
        let getComponents () =
            http.GetFromJsonAsync<string list> "resources/components.json"

        let cmd = Cmd.OfTask.either getComponents () Msg.GotComponents Msg.Error

        model, cmd
    | Msg.GotComponents components -> AddComponentView.``open`` model components, Cmd.none
    | Msg.GotComponent graph -> AddComponentView.importComponent model graph, Cmd.none
    | Msg.SetComponentSelection c -> { model with newComponentSelection = c }, Cmd.none
    | Msg.ModifiedComponentName name -> { model with componentNameInput = name }, Cmd.none
    | Msg.ImportSelectedComponent ->
        let getComponent () =
            http.GetFromJsonAsync<AAG.Component>(
                "resources/aagc/"
                + model.newComponentSelection
            )

        let cmd = Cmd.OfTask.either getComponent () Msg.GotComponent Msg.Error
        model, cmd
    // ModifyComponent
    | Msg.OpenModifyComponent name -> ModifyComponentView.``open`` model name, Cmd.none
    | Msg.ClickedDeleteComponent name -> ModifyComponentView.deleteComponent model name, Cmd.none
    | Msg.ToggleOptionalAccessMethod (b, accessMethod) ->
        { model with
            graph =
                AAG.setDisableEdgesByComponentNameAndAccessName
                    model.graph
                    model.subjectComponentName
                    accessMethod.name
                    (not b) },
        Cmd.none
    | Msg.ToggleCondition (b, condition) ->
        { model with
            graph =
                AAG.setConditionByComponentNameAndConditionName model.graph model.subjectComponentName condition.name b },
        Cmd.none
    // Vinit
    | Msg.OpenVinit -> VinitView.``open`` model, Cmd.none
    | Msg.SetVinit vertex -> VinitView.setVinit vertex true model, Cmd.none
    | Msg.UnsetVinit vertex -> VinitView.setVinit vertex false model, Cmd.none
    // Model
    | Msg.UpdateCompromisedVertices vertexIds -> updateTransitivelyCompromisedVertexIds model vertexIds, Cmd.none

/// The main `view` function of the Elmish program.
/// Populates the HTML templates with the content of the model.
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
        .DebugText(getDebugText model)
        .PhysicsCheckbox(model.physics, (fun b -> dispatch (Msg.SetPhysics b)))
        .EdgeLabelsCheckbox(model.edgeLabels, (fun b -> dispatch (Msg.SetEdgeLabels b)))
        .JsonOutput(model.json, (fun jsonAsString -> dispatch (Msg.ModifiedGraphJson jsonAsString)))
        .JsonOutputHint((getJsonOutputHint model).value)
        .LeftColumn(
            cond model.page
            <| function
                | Endpoint.Main ->
                    match model.view with
                    | MainMenu -> MainMenuView.view dispatch
                    | ModifyAccess -> ModifyAccessView.view jsRuntime model dispatch
                    | ModifyVertex -> ModifyVertexView.view jsRuntime model dispatch
                    | AnalysisManual -> AnalysisManualView.view model dispatch
                    | AnalysisAutomated -> AnalysisAutomatedView.view model dispatch
                    | Vinit -> VinitView.view model dispatch
                    | AddComponent -> AddComponentView.view jsRuntime model dispatch
                    | ModifyComponent -> ModifyComponentView.view model dispatch
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
        Program.mkProgram init (update this.HttpClient) (view this.JSRuntime)
        |> Program.withRouter router
