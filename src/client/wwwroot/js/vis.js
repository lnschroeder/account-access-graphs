function getCssVariable(variable) {
  return getComputedStyle(document.body).getPropertyValue(variable).trim();
}

let initNodes = [
  // { id: 1, label: "abc" },
];
let initEdges = [];

let container = document.getElementById("graph-container");
let data = {
  nodes: new vis.DataSet(initNodes),
  edges: new vis.DataSet(initEdges),
};

let options = {
  layout: { randomSeed: 2 },
  nodes: {
    borderWidth: 1,
    borderWidthSelected: 4,
    color: {
      border: getCssVariable("--bulma-link"),
      background: "#00000000",
      highlight: {
        border: getCssVariable("--bulma-link"),
        background: "#00000000",
      },
    },
    labelHighlightBold: false,
    font: {
      color: getCssVariable("--bulma-text"),
      face: getCssVariable("--bulma-body-family"),
    },
  },
};
let network = new vis.Network(container, data, options);

// removes, updates, and adds nodes and edges to the network
function updateNetwork(nodes, edges) {
  let oldNodeIds = data.nodes.map((item) => item.id);
  let oldEdgeIds = data.edges.map((item) => item.id);
  let newNodeIds = nodes.map((item) => item.id);
  let newEdgeIds = edges.map((item) => item.id);
  let removedNodeIds = [];
  let removedEdgeIds = [];

  oldNodeIds.forEach((id) => {
    if (!newNodeIds.includes(id)) {
      removedNodeIds.push(id);
    }
  });

  oldEdgeIds.forEach((id) => {
    if (!newEdgeIds.includes(id)) {
      removedEdgeIds.push(id);
    }
  });
  data.edges.remove(removedEdgeIds);
  data.nodes.remove(removedNodeIds);
  data.edges.update(edges);
  data.nodes.update(nodes);
}
window.updateNetwork = updateNetwork;
