module AAG.Client.MainMenuStep

open Model

let clearGraph = Model.Init

let exampleGraph = Model.Example

let openAddVertex model =
    let vertex = AAG.Vertex.Default ""

    { model with step = ModifyVertex ; subjectVertexId = Some vertex.id ; graph = AAG.addVertex vertex model.graph}

let openModifyVertex (vertex: AAG.Vertex) model =
    { model with
        step = ModifyVertex
        subjectVertexId = Some vertex.id
        modifyVertexNameInput = vertex.name }

let handleClickedVertex (vertex: AAG.Vertex option) (model: Model) =
    match vertex with
    | Some vertex -> openModifyVertex vertex model
    | None -> model

let view dispatch =
    Template
        .MainMenu()
        .AddVertexButton(fun _ -> dispatch (Msg.ClickedAddVertex))
        .ClearGraphButton(fun _ -> dispatch (Msg.ClickedClearGraph))
        .ExampleGraphButton(fun _ -> dispatch (Msg.ClickedExampleGraph))
        .Elt()
