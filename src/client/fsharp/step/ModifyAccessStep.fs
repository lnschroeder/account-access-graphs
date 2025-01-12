module AAG.Client.ModifyAccessStep

open Model
open Bolero.Html

// TODO add check if colors are correct

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

let private getAccessNameInputPlaceholder (model: Model) =
    $"Defaults to: {model.subjectAccessName}"

let removeFactorFromSubject vertexId (access: AAG.Access) (model: Model) =
    { model with
        graph = AAG.removeVertexFromAccess vertexId access model.graph
        selectedFactors = Set.remove vertexId model.selectedFactors }

let addFactorToSubjectAccess vertexId (access: AAG.Access) (model: Model) =
    { model with
        graph = AAG.addEdge (AAG.Edge.New vertexId access) model.graph
        selectedFactors = Set.add vertexId model.selectedFactors }

let toggleFactorForSubject (vertex: AAG.Vertex) (model: Model) =
    match model.subjectVertexId with
    | Some subjectVertexId ->
        match model.subjectAccessColor with
        | Some color ->
            let access: AAG.Access =
                { name = model.subjectAccessName
                  colorIndex = color
                  vertexId = subjectVertexId }

            if Some vertex.id = model.subjectVertexId then
                model
            elif Set.contains vertex.id model.selectedFactors then
                removeFactorFromSubject vertex.id access model
            else
                addFactorToSubjectAccess vertex.id access model
        | None -> model
    | None -> model

let exitDeletingAccess access model =
    { model with
        selectedFactors = Set.empty
        addAccessNameInput = ""
        graph =
            match access with
            | Some access -> AAG.deleteAccess access model.graph
            | None -> model.graph
        step = ModifyVertex }

let private isValidAccess (model: Model) =
    (getFactorsHint model).level <> Error
    && (getAccessNameHint model).level <> Error

let handleClickedVertex (vertex: AAG.Vertex) (model: Model) dispatch =
    dispatch (Msg.ToggleFactorForSubject(vertex))

let handleClickedBackground model dispatch =
    if isValidAccess model then
        dispatch (Msg.OpenModifyVertexStep model.subjectVertexId)
    else
        dispatch Msg.IgnoreAction

let updateAccessName (access: AAG.Access option) name (model: Model) =
    match model.subjectVertexId with
    | Some subjectVertexId ->
        let model = { model with addAccessNameInput = name }

        if (getAccessNameHint model).level <> Error then
            let subjectAccessName =
                if name = "" then
                    AAG.findNextAvailableName subjectVertexId model.graph
                else
                    model.subjectAccessName

            { model with
                graph =
                    match access with
                    | Some access -> AAG.changeAccessName access subjectAccessName model.graph
                    | None -> model.graph
                subjectAccessName = subjectAccessName }
        else
            model
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

let ``open`` (access: AAG.Access) addAccessNameInput model =
    // match access with
    // | Some access ->
    match AAG.findVertexById access.vertexId model.graph with
    | Some subjectVertex ->
        { page = Endpoint.Main
          step = ModifyAccess
          graph = model.graph
          addAccessNameInput = addAccessNameInput
          modifyVertexNameInput = subjectVertex.name
          subjectVertexId = Some subjectVertex.id
          subjectAccessColor = Some access.colorIndex
          subjectAccessName = access.name
          highlightedEdgeIds = Set.empty
          selectedFactors =
            AAG.findEdgesOfAccess model.graph access
            |> List.map (fun e -> e.from)
            |> Set.ofList
          initiallyCompromisedVertexIds = Set.empty
          transitivelyCompromisedVertexIds = Set.empty
          json = model.json }
    | None -> model
// | None ->
//     match model.subjectVertexId with
//     | Some subjectVertexId ->
//         match AAG.findVertexById subjectVertexId model.graph with
//         | Some subjectVertex ->


//             openModifyAccess subjectVertex access "" model
//         | None -> model
//     | None -> model

let view jsRuntime (model: Model) dispatch =
    Utility.toggleButtonEnabled "ModifyAccessBackButton" (isValidAccess model) jsRuntime
    |> ignore

    match model.subjectVertexId with
    | Some subjectVertexId ->
        match AAG.findVertexById subjectVertexId model.graph with
        | Some subjectVertex ->
            match model.subjectAccessColor with
            | Some subjectAccessColor ->
                let subjectAccess =
                    AAG.findAccessByVertexIdAndColor subjectVertex.id subjectAccessColor model.graph

                Template
                    .ModifyAccess()
                    .DeleteButton(fun _ -> dispatch (Msg.ClickedDeleteAccess subjectAccess))
                    .BackButton(fun _ -> dispatch (Msg.OpenModifyVertexStep model.subjectVertexId))
                    .AccessNameInput(
                        model.addAccessNameInput,
                        (fun v -> dispatch (Msg.ModifiedAccessName(subjectAccess, v)))
                    )
                    .AccessNameInputPlaceholder(getAccessNameInputPlaceholder model)
                    .AccessNameHint((getAccessNameHint model).value)
                    .Factors(forEach model.selectedFactors (showFactor model dispatch))
                    .FactorsHint((getFactorsHint model).value)
                    .Elt()
            | None -> Template.ModifyAccess().Elt()
        | None -> Template.ModifyAccess().Elt()
    | None -> Template.ModifyAccess().Elt()
