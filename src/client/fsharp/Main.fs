module AAG.Client.Main

open System.Net.Http
open Microsoft.AspNetCore.Components
open Elmish
open Bolero
open Model
open Microsoft.JSInterop
open System
open Bolero.Html

let private init _ = MainMenuPage.clearGraph, Cmd.none

let private findVertexById idAsString graph =
    if idAsString = "undefined" then
        None
    else
        let guid = Guid.Parse idAsString
        AAG.findVertexById guid graph

let private update message model =
    let model =
        match message with
        // Main
        | Msg.SetPage page -> { model with page = page }
        | Msg.ClickedVisNode idAsString ->
            let vertex = findVertexById idAsString model.graph

            match model.step with
            | AddAccessSubject -> AddAccessPage.handleClickedVertexOnSubject vertex model
            | AddAccessFactors -> AddAccessPage.handleClickedVertexOnFactors vertex model
            | _ -> model

        // MainMenuPage
        | Msg.ClickedClearGraph -> MainMenuPage.clearGraph
        | Msg.ClickedExampleGraph -> MainMenuPage.exampleGraph
        | Msg.ClickedAddAccess -> MainMenuPage.openAddAccess model
        | Msg.ClickedAddVertex -> MainMenuPage.openAddVertex model
        // AddVertexPage
        | Msg.ClickedSaveAddVertex -> AddVertexPage.addNewVertex model
        | Msg.TypedVertexName value -> AddVertexPage.updateVertexName value model
        | Msg.ClickedCancelAddVertex -> AddVertexPage.cancel model
        // AddAccessPage
        | Msg.TypedAccessName name -> AddAccessPage.updateAccessName name model
        | Msg.ClickedSaveAddAccess name -> AddAccessPage.addNewAccess name model
        | Msg.ClickedBackFromFactors -> AddAccessPage.openSubjectSelection model
        | Msg.TypedSubjectName name -> AddAccessPage.selectSubjectByName name model
        | Msg.ClickedCancelAddAccess -> AddAccessPage.cancel model
        | Msg.ClickedRemoveProvisionalFactor id -> AddAccessPage.removeProvisionalFactor id model
        | Msg.ClickedContinueSubject -> AddAccessPage.openFactorsSelection model

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
                    | Main ->
                        Template
                            .MainMenu()
                            .AddVertexButton(fun _ -> dispatch (Msg.ClickedAddVertex))
                            .AddAccessButton(fun _ -> dispatch (Msg.ClickedAddAccess))
                            .ClearGraphButton(fun _ -> dispatch (Msg.ClickedClearGraph))
                            .ExampleGraphButton(fun _ -> dispatch (Msg.ClickedExampleGraph))
                            .Elt()
                    | AddVertex -> AddVertexPage.view jsRuntime model dispatch
                    | AddAccessSubject -> AddAccessPage.view jsRuntime model dispatch
                    | AddAccessFactors -> AddAccessPage.view jsRuntime model dispatch
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
