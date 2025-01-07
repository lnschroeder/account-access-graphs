module AAG.Client.Json

open System.Text.Json

let serializeGraph (graph: AAG.Graph) =
    use jDoc = JsonDocument.Parse(JsonSerializer.Serialize graph)
    JsonSerializer.Serialize(jDoc, JsonSerializerOptions(WriteIndented = true))

let deserializeGraph (string: string) : AAG.Graph = JsonSerializer.Deserialize string
