module AAG.Client.Template

open Bolero

type Main = Template<"wwwroot/templates/main.html">
type MainMenu = Template<"wwwroot/templates/main-menu.html">
type ModifyAccess = Template<"wwwroot/templates/modify-access.html">
type ModifyVertex = Template<"wwwroot/templates/modify-vertex.html">
type AnalysisCompromise = Template<"wwwroot/templates/analysis-compromise.html">
type AnalysisScore = Template<"wwwroot/templates/analysis-score.html">
type Vinit = Template<"wwwroot/templates/vinit.html">
