module AAG.Client.Json

open System.Text.Json

let serializeGraph (graph: AAG.Graph) =
    use jDoc = JsonDocument.Parse(JsonSerializer.Serialize graph)
    JsonSerializer.Serialize(jDoc, JsonSerializerOptions(WriteIndented = true))

let tryDeserializeGraph (string: string) : Model.DeserializedGraph =
    try
        JsonSerializer.Deserialize string |> Model.DeserializedGraph.Graph
    with
    | :? System.ArgumentNullException -> Model.DeserializedGraph.ErrorMsg (Model.Hint.Error "JSON is null")
    | :? JsonException -> Model.DeserializedGraph.ErrorMsg (Model.Hint.Error "JSON is not a valid AAG format")

