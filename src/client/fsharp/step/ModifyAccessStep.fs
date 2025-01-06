module AAG.Client.ModifyAccessStep

open Model
open Bolero.Html

let private getFactorsHint (model: Model) = // TODO just pass minimal
    match model.subjectVertexId with
    | Some subjectVertexId ->
        if model.selectedFactors.IsEmpty then
            Hint.Error "Select at least one factor"
        elif Seq.contains subjectVertexId model.selectedFactors then
            Hint.Error "Self-references are not allowed"
        elif Seq.length (AAG.getAccessesWithFactors subjectVertexId model.selectedFactors model.graph) > 1 then
            Hint.Error "There is already an access with the same factors"
        else
            Hint.Info
    | None -> Hint.Error "No subject vertex selected yet"

let private getAccessNameHint (model: Model) =
    match model.subjectVertexId with
    | Some subjectVertexId ->
        if model.addAccessNameInput = "" then
            Hint.Info
        elif AAG.isInvalidAccessName model.addAccessNameInput then
            Hint.Error "Invalid name"
        elif Seq.length (AAG.getAccessesWithName subjectVertexId model.addAccessNameInput model.graph) > 1 then
            Hint.Error "Access name already taken for that vertex"
        else
            Hint.Info
    | None -> Hint.Error "Select a subject vertex first!"

let private getAccessNameInputPlaceholder (model: Model) =
    match model.subjectVertexId with
    | Some subjectVertexId ->
        match AAG.findVertexById subjectVertexId model.graph with
        | Some subject ->
            "Defaults to: "
            + (AAG.findNextAvailableColor subject).ToString()
        | None -> "Invalid subject vertex selected"
    | None -> "Select a subject vertex first"

// let private deselectSubject (model: Model) =
//     { model with
//         subjectId = None
//         graph = AAG.removeAllProvisionalAccesses model.graph }

// let private selectSubject (vertex: AAG.Vertex) (model: Model) =
//     { model with
//         subjectId = Some vertex.id
//         graph =
//             AAG.setProvisionalAccess
//                 vertex.id
//                 model.addAccessNameInput
//                 (Set.ofList model.factorsInput)
//                 (AAG.removeAllProvisionalAccesses model.graph) }

// let removeProvisionalFactor vertexId (model: Model) =
//     match model.subjectVertexId with
//     | Some subjectVertexId ->
//         let factors =
//             model.selectedFactors
//             |> List.filter (fun id -> id <> vertexId)

//         { model with
//             factorsInput = factors
//             graph = AAG.setProvisionalAccess subjectVertexId model.addAccessNameInput (Set.ofList factors) model.graph }
//     | None -> model

// let addProvisionalFactor vertexId (model: Model) =
//     match model.subjectVertexId with
//     | Some subjectVertexId ->
//         let factors = model.factorsInput @ [ vertexId ]

//         { model with
//             factorsInput = factors
//             graph = AAG.setProvisionalAccess subjectVertexId model.addAccessNameInput (Set.ofList factors) model.graph }
//     | None -> model

let removeFactorFromSubject vertexId (model: Model) =
    match model.subjectAccessId with
    | Some subjectAccessId ->
        { model with
            graph = AAG.removeFactorFromGraph subjectAccessId vertexId model.graph
            selectedFactors = Set.remove vertexId model.selectedFactors }
    | None -> model

let addFactorFromSubject vertexId (model: Model) =
    match model.subjectAccessId with
    | Some subjectAccessId ->
        { model with
            graph = AAG.addFactorToGraph subjectAccessId vertexId model.graph
            selectedFactors = Set.add vertexId model.selectedFactors }
    | None -> model

let toggleFactor (vertex: AAG.Vertex) (model: Model) =
    if Some vertex.id = model.subjectVertexId then
        model
    elif Set.contains vertex.id model.selectedFactors then
        removeFactorFromSubject vertex.id model
    else
        addFactorFromSubject vertex.id model

let exitDeletingProvisionalAccesses model =
    { model with
        selectedFactors = Set.empty
        addAccessNameInput = ""
        graph = AAG.removeAllProvisionalAccesses model.graph
        step = ModifyVertex }

let handleClickedVertex (vertex: AAG.Vertex option) (model: Model) dispatch =
    match vertex with
    | Some vertex -> dispatch (Msg.ToggleFactorOfSubjectAccess vertex)
    | _ -> dispatch Msg.IgnoreAction

let private isValidNewAccess (model: Model) =
    (getFactorsHint model).level <> Error
    && (getAccessNameHint model).level <> Error

let updateAccessName subjectAccessId name (model: Model) =
    { model with
        graph = AAG.changeAccessName subjectAccessId name model.graph
        addAccessNameInput = name }

let private showFactor (model: Model) dispatch id =
    match AAG.findVertexById id model.graph with
    | Some vertex ->
        Template
            .ModifyAccess
            .Factor()
            .Name(vertex.name)
            .DeleteButton(fun _ -> dispatch (Msg.ClickedRemoveProvisionalFactor vertex.id))
            .Elt()
    | None ->
        Template
            .ModifyAccess
            .Factor()
            .Name("INVALID")
            .DeleteButton(fun _ -> ())
            .Elt()

let private openModifyAccess (vertex: AAG.Vertex) (access: AAG.Access) model =
    match AAG.findVertexById vertex.id model.graph with
    | Some subjectVertex ->
        { page = Endpoint.Main
          step = ModifyAccess
          graph = model.graph
          addAccessNameInput = access.name
          modifyVertexNameInput = subjectVertex.name
          subjectVertexId = Some subjectVertex.id
          subjectAccessId = Some access.id
          selectedFactors =
            access.factors
            |> Set.map (fun factor -> factor.vertexId) }
    | None -> model // TODO throw error if the vertex is invalid

let private createNewAccessForSubjectVertex (model: Model) =
    match model.subjectVertexId with
    | Some subjectVertexId ->
        match AAG.findVertexById subjectVertexId model.graph with
        | Some subjectVertex ->
            let access = AAG.Access.New(AAG.findNextAvailableColor subjectVertex)
            openModifyAccess subjectVertex access model
        | None -> model
    | None -> model

let ``open`` accessId model =
    match accessId with
    | Some accessId ->
        match AAG.findAccessById accessId model.graph with
        | Some (vertex, access) -> openModifyAccess access vertex model
        | None -> model
    | None -> createNewAccessForSubjectVertex model

let view jsRuntime (model: Model) dispatch =
    Utility.toggleButtonEnabled "ModifyAccessBackButton" (isValidNewAccess model) jsRuntime
    |> ignore

    match model.subjectAccessId with
    | Some subjectAccessId ->
        Template
            .ModifyAccess()
            .DeleteButton(fun _ -> dispatch (Msg.ClickedDeleteAccess))
            .BackButton(fun _ -> dispatch (Msg.OpenModifyVertexStep model.subjectVertexId))
            .AccessNameInput(model.addAccessNameInput, (fun v -> dispatch (Msg.ModifiedAccessName (subjectAccessId, v))))
            .AccessNameInputPlaceholder(getAccessNameInputPlaceholder model)
            .AccessNameHint((getAccessNameHint model).value)
            .Factors(forEach model.selectedFactors (showFactor model dispatch))
            .FactorsHint((getFactorsHint model).value)
            .Elt()
    | None -> Template.ModifyAccess().Elt()
