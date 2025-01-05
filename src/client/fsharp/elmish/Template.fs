module AAG.Client.Template

open Bolero

type Main = Template<"wwwroot/main.html">
type MainMenu = Template<"wwwroot/main-menu.html">
type AddVertex = Template<"wwwroot/add-vertex.html">
type AddAccessSubject = Template<"wwwroot/add-access_subject.html">
type AddAccessFactors = Template<"wwwroot/add-access_factors.html">
type ModifyVertex = Template<"wwwroot/modify-vertex.html">
