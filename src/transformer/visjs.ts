import type {Graph} from "../domain/model";
import {type Data, Network} from "vis-network";

export function getNetwork(container: HTMLElement, graph: Graph): Network {
  return new Network(container, transformGraph(graph), options())
}

const options = () => ({
  layout: {randomSeed: 2},
  nodes: {
    labelHighlightBold: false,
    borderWidth: 1,
    borderWidthSelected: 1,
    font: {
      // face: getCssVariable("--bulma-body-family"),
      multi: "md",
    },
  },
  edges: {
    arrows: "to",
    arrowStrikethrough: false,
  },
  interaction: {
    selectable: false,
    selectConnectedEdges: false,
    hoverConnectedEdges: false,
  },
});


function transformGraph(graph: Graph): Data {
  return {
    nodes: graph.nodes,
    edges: graph.edges,
  };
}
