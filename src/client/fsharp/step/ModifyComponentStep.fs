module AAG.Client.ModifyComponentStep

open Model
open Bolero.Html

// Validity

// Handle actions
let handleClickedBackground model dispatch =
    dispatch (Msg.OpenModifyVertexStep model.subjectVertexId)

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

let private showPolicyInfo graph componentName (rule: AAG.Rule) =
    let result = AAG.evaluateAuthenticationPolicyRule graph componentName rule
    if not result then
        Template
            .ModifyComponent
            .SomePolicyInfo()
            .Description(rule.description)
            .Elt()
    else
        Template
            .ModifyComponent
            .NonePolicyInfo()
            .Elt()

let view (model: Model) dispatch =
    let subjectComponent = AAG.getComponent model.graph model.subjectComponentName
    Template
        .ModifyComponent()
        .ComponentName(model.subjectComponentName)
        .Conditions(forEach (subjectComponent.conditions) (showCondition dispatch))
        .AccessMethods(forEach (subjectComponent.accessMethods) (showAccessMethod dispatch))
        .PolicyInfos(forEach (subjectComponent.rules) (showPolicyInfo model.graph model.subjectComponentName))
        .DeleteButton(fun _ -> dispatch (Msg.ClickedDeleteComponent model.subjectComponentName))
        .BackButton(fun _ -> dispatch (Msg.OpenModifyVertexStep model.subjectVertexId))
        .Elt()
