module AAG.Client.ModifyAccessStep

open Model
open Bolero.Html

let private getFactorsHint (model: Model) = // TODO just pass minimal
    match model.subjectVertexId with
    | Some subjectVertexId ->
        if model.selectedFactors.IsEmpty then
            Hint.Error "Select at least one factor or delete the access"
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
        let name = model.addAccessNameInput

        if name = "" then
            Hint.Info
        elif AAG.isInvalidAccessName name then
            Hint.Error "Invalid name"
        elif Seq.length (AAG.getAccessesWithName subjectVertexId name model.graph) > 1 then
            Hint.Error "Access name already taken for that vertex"
        else
            Hint.Info
    | None -> Hint.Error "Select a subject vertex first!"

let private getAccessNameInputPlaceholder subjectAccessId (model: Model) =
    match AAG.findAccessById subjectAccessId model.graph with
    | Some (_, access) -> $"Defaults to: {access.name}"
    | None -> "Error"

let removeFactorFromSubject accessId vertexId (model: Model) =
    { model with
        graph = AAG.removeFactorFromGraph accessId vertexId model.graph
        selectedFactors = Set.remove vertexId model.selectedFactors }

let addFactorFromSubject accessId vertexId (model: Model) =
    { model with
        graph = AAG.addFactorToGraph accessId vertexId model.graph
        selectedFactors = Set.add vertexId model.selectedFactors }

let toggleFactor accessId (vertex: AAG.Vertex) (model: Model) =
    if Some vertex.id = model.subjectVertexId then
        model
    elif Set.contains vertex.id model.selectedFactors then
        removeFactorFromSubject accessId vertex.id model
    else
        addFactorFromSubject accessId vertex.id model

let exitDeletingAccess subjectAccessId model =
    { model with
        selectedFactors = Set.empty
        addAccessNameInput = ""
        graph = AAG.deleteAccess subjectAccessId model.graph
        step = ModifyVertex }

let private isValidNewAccess (model: Model) =
    (getFactorsHint model).level <> Error
    && (getAccessNameHint model).level <> Error

let handleClickedVertex (vertex: AAG.Vertex) (model: Model) dispatch =
    match model.subjectAccessId with
    | Some subjectAccessId -> dispatch (Msg.ToggleFactorOfSubjectAccess(subjectAccessId, vertex))
    | None -> dispatch Msg.IgnoreAction

let handleClickedBackground (model: Model) dispatch =
    if isValidNewAccess model then
        dispatch (Msg.OpenModifyVertexStep model.subjectVertexId)
    else
        dispatch Msg.IgnoreAction

let updateAccessName (access: AAG.Access) name (model: Model) =
    match model.subjectVertexId with
    | Some subjectVertexId ->
        match AAG.findVertexById subjectVertexId model.graph with
        | Some subjectVertex ->
            if name = "" then
                let name = AAG.findNextAvailableName subjectVertex
                { model with graph = AAG.changeAccessName access.id name model.graph }
            else
                { model with
                    graph = AAG.changeAccessName access.id name model.graph
                    addAccessNameInput = name }
        | None -> model
    | None -> model

let private showFactor (model: Model) dispatch id =
    match AAG.findVertexById id model.graph with
    | Some vertex ->
        Template
            .ModifyAccess
            .Factor()
            .Name(vertex.name)
            .Elt()
    | None ->
        Template
            .ModifyAccess
            .Factor()
            .Name("INVALID")
            .Elt()

let private openModifyAccess vertexId (access: AAG.Access) accessNameInput model =
    match AAG.findVertexById vertexId model.graph with
    | Some subjectVertex ->
        { page = Endpoint.Main
          step = ModifyAccess
          graph = model.graph
          addAccessNameInput = accessNameInput
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
            let name = AAG.findNextAvailableName subjectVertex
            printfn "name  %A" name
            let access = AAG.Access.New name (AAG.findNextAvailableColor subjectVertex)
            let graph = AAG.addAccessToGraph subjectVertexId access model.graph

            openModifyAccess subjectVertex.id access "" { model with graph = graph }
        | None -> model
    | None -> model

let ``open`` accessId model =
    match accessId with
    | Some accessId ->
        match AAG.findAccessById accessId model.graph with
        | Some (vertexId, access) -> openModifyAccess vertexId access access.name model
        | None -> model
    | None -> createNewAccessForSubjectVertex model

let view jsRuntime (model: Model) dispatch =
    Utility.toggleButtonEnabled "ModifyAccessBackButton" (isValidNewAccess model) jsRuntime
    |> ignore

    match model.subjectVertexId with
    | Some subjectVertexId ->
        match AAG.findVertexById subjectVertexId model.graph with
        | Some subjectVertex ->
            match model.subjectAccessId with
            | Some subjectAccessId ->
                match AAG.findAccessById subjectAccessId model.graph with
                | Some (_, subjectAccess) ->
                    Template
                        .ModifyAccess()
                        .DeleteButton(fun _ -> dispatch (Msg.ClickedDeleteAccess subjectAccessId))
                        .BackButton(fun _ -> dispatch (Msg.OpenModifyVertexStep model.subjectVertexId))
                        .AccessNameInput(
                            model.addAccessNameInput,
                            (fun v -> dispatch (Msg.ModifiedAccessName(subjectAccess, v)))
                        )
                        .AccessNameInputPlaceholder(getAccessNameInputPlaceholder subjectAccessId model)
                        .AccessNameHint((getAccessNameHint model).value)
                        .Factors(forEach model.selectedFactors (showFactor model dispatch))
                        .FactorsHint((getFactorsHint model).value)
                        .Elt()
                | None -> Template.ModifyAccess().Elt()
            | None -> Template.ModifyAccess().Elt()
        | None -> Template.ModifyAccess().Elt()
    | None -> Template.ModifyAccess().Elt()
