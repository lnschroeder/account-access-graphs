module AAG.Client.Endpoint

open Bolero

/// Routing endpoints definition.
type Page = | [<EndPoint "/">] Graph
