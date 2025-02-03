module AAG.Client.ModifyComponentStep

open Model
open Bolero.Html

// Validity

// Handle actions
let handleClickedVertex (vertex: AAG.Vertex) (model: Model) dispatch =
    dispatch Msg.IgnoreAction

let handleClickedBackground (model: Model) dispatch =
    Msg.IgnoreAction

let handleClickedEdge (access: AAG.Access) (model: Model) dispatch =
    dispatch Msg.IgnoreAction

// Open
let ``open`` model componentName =
    { page = model.page
      step = ModifyComponent
      physics = model.physics
      edgeLabels = model.edgeLabels
      graph = model.graph
      addAccessNameInput = model.addAccessNameInput
      modifyVertexNameInput = model.modifyVertexNameInput
      subjectVertexId = model.subjectVertexId
      subjectAccessColor = None
      subjectAccessName = model.subjectAccessName
      selectedFactors = Set.empty
      highlightedEdgeIds = Set.empty
      initiallyCompromisedVertexIds = Set.empty
      transitivelyCompromisedVertexIds = Set.empty
      subjectComponentName = componentName
      newComponentSelection = ""
      components = []
      componentNameInput = ""
      json = model.json }

// Functionality
let deleteComponent (model: Model) componentName =
    { model with
        step = MainMenu
        subjectVertexId = None
        newComponentSelection = model.subjectComponentName
        graph = AAG.deleteComponent model.graph componentName}

// View
let private showCondition dispatch (condition: AAG.Condition) =
    Template
        .ModifyComponent
        .Question()
        .EnabledInput(condition.answer, (fun b -> dispatch (Msg.ToggleCondition (b, condition))))
        .Description(condition.description)
        .Elt()

let private showAccessMethod dispatch (accessMethod: AAG.OptionalAccessMethod) =
    Template
        .ModifyComponent
        .Question()
        .EnabledInput(accessMethod.answer, (fun b -> dispatch (Msg.ToggleOptionalAccessMethod (b, accessMethod))))
        .Description(accessMethod.description)
        .Elt()

let view jsRuntime (model: Model) dispatch =
    Template
        .ModifyComponent()
        // .SubjectNameInput(
        //     model.modifyVertexNameInput,
        //     (fun v -> dispatch (Msg.ModifiedVertexName(subjectVertexId, v)))
        // )
        // .SubjectNameHint((modifyVertexNameInput model).value)
        .ComponentName(model.subjectComponentName)
        .Conditions(forEach ((AAG.getComponent model.graph model.subjectComponentName).conditions) (showCondition dispatch))
        .AccessMethods(forEach ((AAG.getComponent model.graph model.subjectComponentName).accessMethods) (showAccessMethod dispatch))
        // .AddAccessButton(fun _ ->
        //     dispatch (
        //         Msg.OpenModifyAccessStep(
        //             { vertexId = subjectVertexId
        //                 colorIndex = AAG.getNextAvailableColor subjectVertex.id model.graph
        //                 name = AAG.getNextAvailableName subjectVertex.id model.graph },
        //             ""
        //         )
        //     ))
        // .VinitInput(
        //     subjectVertex.isVinit,
        //     (fun b ->
        //         if b then
        //             dispatch (Msg.SetVinit subjectVertex)
        //         else
        //             dispatch (Msg.UnsetVinit subjectVertex))
        // )
        // .ScoreInput(subjectVertex.score, (fun i -> dispatch (Msg.SetScore(subjectVertex, i))))
        // .ComponentInfo(
        //     match subjectVertex.``component`` with
        //     | Some c ->
        //         Template
        //             .ModifyVertex
        //             .SomeComponentInfo()
        //             .ComponentName(c)
        //             .ModifyComponentButton(fun _ -> dispatch Msg.OpenMainMenuStep)
        //             .Elt()
        //     | None -> Template.ModifyVertex.NoneComponentInfo().Elt()
        // )
        .DeleteButton(fun _ -> dispatch (Msg.ClickedDeleteComponent model.subjectComponentName))
        .BackButton(fun _ -> dispatch (Msg.OpenModifyVertexStep model.subjectVertexId))
        .Elt()
