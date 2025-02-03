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
open Newtonsoft.Json.Linq
open JsonLogic.Net

let private init _ = MainMenuStep.clearGraph, Cmd.none

// TODO this function is just for testing purposes
let private testAuthenticationPolicyEvaluation =
    let evaluator = JsonLogicEvaluator(EvaluateOperators.Default)

    let ruleObj =
        JObject.Parse(
            """
        {
            "if": [
                { "var": "passkey" },
                { "var": "isPremium" },
                true
            ]
        }
        """
        )

    let data = {| passkey = false; isPremium = true |}
    let result = evaluator.Apply(ruleObj, data)
    printfn "%A" result

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
    | MainMenu -> MainMenuStep.handleClickedBackground dispatch
    | ModifyVertex -> ModifyVertexStep.handleClickedBackground model dispatch
    | ModifyAccess -> ModifyAccessStep.handleClickedBackground model dispatch
    | AnalysisManual -> AnalysisManualStep.handleClickedBackground model dispatch
    | AnalysisAutomated -> AnalysisAutomatedStep.handleClickedBackground dispatch
    | Vinit -> VinitStep.handleClickedBackground dispatch
    | AddComponent -> dispatch Msg.IgnoreAction
    | ModifyComponent -> dispatch Msg.IgnoreAction

let private handleClickedVertex (idAsString: string) model dispatch =
    let vertex =
        match Guid.TryParse(idAsString) with
        | (true, guid) -> AAG.tryFindVertexById guid model.graph
        | (false, _) -> None

    match vertex with
    | Some vertex ->
        match model.step with
        | MainMenu -> MainMenuStep.handleClickedVertex vertex dispatch
        | ModifyVertex -> ModifyVertexStep.handleClickedVertex vertex model dispatch
        | ModifyAccess -> ModifyAccessStep.handleClickedVertex vertex dispatch
        | AnalysisManual -> AnalysisManualStep.handleClickedVertex model vertex dispatch
        | AnalysisAutomated -> AnalysisAutomatedStep.handleClickedVertex vertex dispatch
        | Vinit -> VinitStep.handleClickedVertex vertex dispatch
        | AddComponent -> dispatch Msg.IgnoreAction
        | ModifyComponent -> dispatch Msg.IgnoreAction
    | None -> dispatch Msg.IgnoreAction

let private handleClickedVisEdge (idAsString: string) model dispatch =
    let access =
        match Guid.TryParse(idAsString) with
        | (true, guid) -> AAG.tryFindAccessByEdgeId guid model.graph
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
            physics = model.physics
            edgeLabels = model.edgeLabels
            json = jsonAsString }
    | ErrorMsg _ -> { model with json = jsonAsString }

let private update (http: HttpClient) message model =
    testAuthenticationPolicyEvaluation

    match message with
    | Msg.IgnoreAction -> model, Cmd.none
    | Msg.Error _ -> model, Cmd.none // TODO
    // Main
    | Msg.SetPage page -> { model with page = page }, Cmd.none
    | Msg.ModifiedGraphJson jsonAsString -> handleUpdatedJson jsonAsString model, Cmd.none
    | Msg.GotGraph graph -> { Model.Init with graph = graph }, Cmd.none
    | Msg.SetPhysics b -> { model with physics = b }, Cmd.none
    | Msg.SetEdgeLabels b -> { model with edgeLabels = b }, Cmd.none
    // MainMenu
    | Msg.OpenMainMenuStep -> MainMenuStep.``open`` model, Cmd.none
    | Msg.ClickedClearGraph -> MainMenuStep.clearGraph, Cmd.none
    | Msg.ClickedExampleGraph -> MainMenuStep.exampleGraph, Cmd.none
    | Msg.ClickedExample2Graph ->
        let getGraph () =
            http.GetFromJsonAsync<AAG.Graph>("resources/hammann-fig14.json")

        let cmd = Cmd.OfTask.either getGraph () Msg.GotGraph Msg.Error
        model, cmd
    // ModifyAccess
    | Msg.OpenModifyAccessStep (access, addAccessNameInput) ->
        ModifyAccessStep.``open`` access addAccessNameInput model, Cmd.none
    | Msg.ModifiedAccessName (access, name) -> ModifyAccessStep.updateAccessName access name model, Cmd.none
    | Msg.ToggleFactorForSubject vertex -> ModifyAccessStep.toggleFactorForSubject vertex model, Cmd.none
    | Msg.ClickedDeleteAccess access -> ModifyAccessStep.exitDeletingAccess access model, Cmd.none
    // ModifyVertex
    | Msg.OpenModifyVertexStep vertexId -> ModifyVertexStep.``open`` vertexId model, Cmd.none
    | Msg.ModifiedVertexName (subjectVertexId, name) ->
        ModifyVertexStep.updateVertexName subjectVertexId name model, Cmd.none
    | Msg.ClickedDeleteVertex subjectVertex -> ModifyVertexStep.exitDeletingVertex subjectVertex model, Cmd.none
    | Msg.HighlightAccess access -> highlightAccess access model, Cmd.none
    | Msg.DeHighlightAccess -> deHighlightAccess model, Cmd.none
    | Msg.SetScore (vertex, score) -> ModifyVertexStep.setScore vertex score model, Cmd.none
    // AnalysisManual
    | Msg.OpenAnalysisManual -> AnalysisManualStep.``open`` model, Cmd.none
    // AnalysisAutomated
    | Msg.OpenAnalysis -> AnalysisAutomatedStep.``open`` model, Cmd.none
    | Msg.OpenAnalysisForVertex vertexId -> AnalysisAutomatedStep.showAnalysisForSubject model vertexId, Cmd.none
    // AddComponent
    | Msg.OpenAddComponentStep ->
        let getComponents () =
            http.GetFromJsonAsync<string list>("resources/components.json")

        let cmd = Cmd.OfTask.either getComponents () Msg.GotComponents Msg.Error

        model, cmd
    | Msg.GotComponents components -> AddComponentStep.``open`` model components, Cmd.none
    | Msg.GotComponent graph -> AddComponentStep.importComponent model graph, Cmd.none
    | Msg.SetComponentSelection c -> { model with newComponentSelection = c }, Cmd.none
    | Msg.ModifiedComponentName name -> { model with componentNameInput = name }, Cmd.none
    | Msg.ImportSelectedComponent ->
        let getComponent () =
            http.GetFromJsonAsync<AAG.Component>("resources/components/" + model.newComponentSelection)

        let cmd = Cmd.OfTask.either getComponent () Msg.GotComponent Msg.Error
        model, cmd
    // ModifyComponent
    | Msg.OpenModifyComponent name -> ModifyComponentStep.``open`` model name, Cmd.none
    | Msg.ClickedDeleteComponent name -> ModifyComponentStep.deleteComponent model name, Cmd.none
    // Vinit
    | Msg.OpenVinit -> VinitStep.``open`` model, Cmd.none
    | Msg.SetVinit vertex -> VinitStep.setVinit vertex true model, Cmd.none
    | Msg.UnsetVinit vertex -> VinitStep.setVinit vertex false model, Cmd.none
    // Model
    | Msg.UpdateCompromisedVertices vertexIds -> updateTransitivelyCompromisedVertexIds model vertexIds, Cmd.none

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
        .PhysicsCheckbox(model.physics, (fun b -> dispatch (Msg.SetPhysics b)))
        .EdgeLabelsCheckbox(model.edgeLabels, (fun b -> dispatch (Msg.SetEdgeLabels b)))
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
                    | AnalysisManual -> AnalysisManualStep.view model dispatch
                    | AnalysisAutomated -> AnalysisAutomatedStep.view model dispatch
                    | Vinit -> VinitStep.view model dispatch
                    | AddComponent -> AddComponentStep.view jsRuntime model dispatch
                    | ModifyComponent -> ModifyComponentStep.view jsRuntime model dispatch
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
