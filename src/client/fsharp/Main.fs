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
    | AddAccessSubject -> AddAccessStep.handleClickedVertexOnSubject vertex model
    | AddAccessFactors -> AddAccessStep.handleClickedVertexOnFactors vertex model
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
        | Msg.ClickedAddAccess -> MainMenuStep.openAddAccess model
        | Msg.ClickedAddVertex -> MainMenuStep.openAddVertex model
        // AddVertexStep
        | Msg.ClickedSaveAddVertex -> AddVertexStep.addNewVertex model
        | Msg.TypedVertexName value -> AddVertexStep.updateVertexName value model
        | Msg.ClickedCancelAddVertex -> AddVertexStep.cancel model
        // AddAccessStep
        | Msg.TypedAccessName name -> AddAccessStep.updateAccessName name model
        | Msg.ClickedSaveAddAccess name -> AddAccessStep.addNewAccess name model
        | Msg.ClickedBackFromFactors -> AddAccessStep.openSubjectSelection model
        | Msg.TypedSubjectName name -> AddAccessStep.selectSubjectByName name model
        | Msg.ClickedCancelAddAccess -> AddAccessStep.cancel model
        | Msg.ClickedRemoveProvisionalFactor id -> AddAccessStep.removeProvisionalFactor id model
        | Msg.ClickedContinueSubject -> AddAccessStep.openFactorsSelection model

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
                    | AddAccessSubject -> AddAccessStep.view jsRuntime model dispatch
                    | AddAccessFactors -> AddAccessStep.view jsRuntime model dispatch
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
