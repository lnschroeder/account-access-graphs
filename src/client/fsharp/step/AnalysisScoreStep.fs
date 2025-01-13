module AAG.Client.AnalysisScoreStep

open Model

// Validity
// Handle actions
// Open
let ``open`` model =
    { page = model.page
      step = AnalysisScore
      graph = AAG.resetScores model.graph
      addAccessNameInput = model.addAccessNameInput
      modifyVertexNameInput = model.modifyVertexNameInput
      subjectVertexId = model.subjectVertexId
      subjectAccessColor = model.subjectAccessColor
      subjectAccessName = model.subjectAccessName
      selectedFactors = Set.empty
      highlightedEdgeIds = Set.empty
      initiallyCompromisedVertexIds = Set.empty
      transitivelyCompromisedVertexIds = Set.empty
      json = model.json }

// Functionality
let computeSumThenMin (model: Model) =
    { model with graph = AAG.recomputeSumThenMinScores model.graph }

// View
let view (model: Model) dispatch =
    Template
        .AnalysisScore()
        .SumThenMinButton(fun _ -> dispatch Msg.ClickedSumThenMinButton)
        .BackButton(fun _ -> dispatch Msg.OpenMainMenuStep)
        .Elt()
