module AAG.Client.Template

open Bolero

type Main = Template<"wwwroot/templates/main.html">
type MainMenu = Template<"wwwroot/templates/main-menu.html">
type ModifyAccess = Template<"wwwroot/templates/modify-access.html">
type ModifyVertex = Template<"wwwroot/templates/modify-vertex.html">
type AnalysisManual = Template<"wwwroot/templates/analysis-manual.html">
type AnalysisAutomated = Template<"wwwroot/templates/analysis-automated.html">
type AddComponent = Template<"wwwroot/templates/add-component.html">
type ModifyComponent = Template<"wwwroot/templates/modify-component.html">
type Vinit = Template<"wwwroot/templates/vinit.html">
