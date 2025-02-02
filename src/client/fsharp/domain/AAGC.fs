module AAG.Client.AAGC

open AAG
open Newtonsoft.Json.Linq
open System

type Component =
    { permissions: string list
      rules: JObject list
      graph: Graph }

let private updateVertexId graph oldId =
    let newId = Guid.NewGuid()

    { vertices =
        graph.vertices
        |> List.map (fun v ->
            if v.id = oldId then
                { v with id = newId }
            else
                v)
      edges =
        graph.edges
        |> List.map (fun e ->
            { e with
                from = if e.from = oldId then newId else e.from
                ``to`` =
                    if e.``to`` = oldId then
                        newId
                    else
                        e.``to`` }) }

let rec private updateVertexIds graph vertexIds =
    match vertexIds with
    | [] -> graph
    | oldId :: rest ->
        let updatedGraph = updateVertexId graph oldId
        updateVertexIds updatedGraph rest

let private updateIds (graph: Graph) =
    let graph =
        { graph with
            edges =
                graph.edges
                |> List.map (fun e -> { e with id = Guid.NewGuid() }) }

    updateVertexIds graph (graph.vertices |> List.map (fun v -> v.id))

let importComponent graph (component: Component) name =
    let updatedGraph = updateIds component.graph

    { vertices =
        List.append
            graph.vertices
            (updatedGraph.vertices
             |> List.map (fun v -> { v with component = Some name }))
      edges = List.append graph.edges updatedGraph.edges }
