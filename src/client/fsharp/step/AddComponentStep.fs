module AAG.Client.AddComponentStep

open Model
open Bolero.Html

// Validity

// Handle actions

// Open
let ``open`` model (components: string list) =
    { page = model.page
      step = AddComponent
      physics = model.physics
      edgeLabels = model.edgeLabels
      graph = model.graph
      addAccessNameInput = ""
      modifyVertexNameInput = ""
      subjectVertexId = None
      subjectAccessColor = None
      subjectAccessName = ""
      selectedFactors = Set.empty
      highlightedEdgeIds = Set.empty
      initiallyCompromisedVertexIds = Set.empty
      transitivelyCompromisedVertexIds = Set.empty
      newComponentSelection = components.Head
      components = components
      componentNameInput = ""
      subjectComponentName = ""
      json = model.json }

// Functionality
let importComponent model (c: AAG.Component) =
    { model with graph = AAG.importComponent model.graph { c with name = model.componentNameInput } }

// View
let private showComponentOption (model: Model) (filename: string) =
    Template
        .AddComponent
        .ComponentOption()
        .FileName(filename)
        .Elt()

let view jsRuntime (model: Model) dispatch =
    Utility.toggleButtonEnabled "ConfirmAddComponentButton" ((componentNameInput model).level = Info) jsRuntime
    |> ignore

    Template
        .AddComponent()
        .Cancel(fun _ -> dispatch Msg.OpenMainMenuStep)
        .Confirm(fun _ -> dispatch Msg.ImportSelectedComponent)
        .ComponentOptions(forEach model.components (showComponentOption model))
        .SelectedOption(model.components.Head, (fun c -> dispatch (Msg.SetComponentSelection c)))
        .ComponentNameInput(model.componentNameInput, (fun v -> dispatch (Msg.ModifiedComponentName v)))
        .ComponentNameHint((componentNameInput model).value)
        .DropDownHint((newComponentHint model).value)
        .Elt()
