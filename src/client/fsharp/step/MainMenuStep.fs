module AAG.Client.MainMenuStep

open Model

let clearGraph = Model.Init

let exampleGraph = Model.Example

let ``open`` model =
    { page = Endpoint.Main
      step = MainMenu
      graph = model.graph
      addAccessNameInput = ""
      modifyVertexNameInput = ""
      subjectVertexId = None
      subjectAccessId = None
      selectedFactors = Set.empty }

let handleClickedVertex (vertex: AAG.Vertex option) (model: Model) dispatch =
    match vertex with
    | Some vertex -> dispatch (Msg.OpenModifyVertexStep (Some vertex.id))
    | None -> dispatch Msg.IgnoreAction

let view dispatch =
    Template
        .MainMenu()
        .AddVertexButton(fun _ -> dispatch (Msg.OpenModifyVertexStep None))
        .ClearGraphButton(fun _ -> dispatch (Msg.ClickedClearGraph))
        .ExampleGraphButton(fun _ -> dispatch (Msg.ClickedExampleGraph))
        .Elt()
