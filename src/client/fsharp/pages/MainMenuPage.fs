module AAG.Client.MainMenuPage

open Model

let clearGraph = Model.Init

let exampleGraph = Model.Example

let openAddVertex model = { model with step = AddVertex }

let openAddAccess model = { model with step = AddAccessSubject }


