// utility
function setButtonDisabled(buttonId, isDisabled) {
  document.getElementById(buttonId).disabled = isDisabled;
}
window.setButtonDisabled = setButtonDisabled;

function getCssVariable(variable) {
  return getComputedStyle(document.body).getPropertyValue(variable).trim();
}

// vis network
let initNodes = [
  // { id: 1, label: "abc" },
];
let initEdges = [];

let container = document.getElementById("graph-container");
let data = {
  nodes: new vis.DataSet(initNodes),
  edges: new vis.DataSet(initEdges),
};

const options = () => ({
  layout: { randomSeed: 2 },
  nodes: {
    labelHighlightBold: false,
    borderWidth: 1,
    borderWidthSelected: 1,
    font: {
      face: getCssVariable("--bulma-body-family"),
    },
  },
  interaction: {
    selectable: false,
    selectConnectedEdges: false,
    hoverConnectedEdges: false,
  },
});
let network = new vis.Network(container, data, options());

// removes, updates, and adds nodes and edges to the network
function updateNetwork(networkDTO) {
  let nodes = networkDTO.nodes;
  let edges = networkDTO.edges;

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

  nodes.forEach((node) => {
    let borderWidth = 1;
    let borderColor = getCssVariable("--bulma-link");
    let backgroundColor = "#00000000";
    let fontColor = getCssVariable("--bulma-text-bold");

    if (node.isSelected) {
      borderWidth = 3;
    }

    if (node.isFactor) {
      backgroundColor = getCssVariable("--bulma-warning-on-scheme");
      fontColor = getCssVariable("--bulma-text-bold-invert");
    }

    data.nodes.update({
      id: node.id,
      borderWidth: borderWidth,
      color: {
        border: borderColor,
        background: backgroundColor,

        highlight: {
          border: borderColor,
          background: backgroundColor,
        },
      },

      font: {
        color: fontColor,
      },
    });
  });
}
window.updateNetwork = updateNetwork;

// unselect dragged node after dragging
network.on("dragEnd", function () {
  network.unselectAll();
});

// custom behavior for clicking on nodes/edges
network.on("click", function (params) {
  // click on node behavior
  let nodeId = this.getNodeAt(params.pointer.DOM);
  let nodeLabel = "";
  let clickedNodeInput = document.getElementById("ClickedNodeInput");

  if (nodeId != undefined) {
    let node = data.nodes.get(nodeId);

    nodeLabel = node.label;
  }

  clickedNodeInput.value = nodeLabel;
  clickedNodeInput.dispatchEvent(new Event("input", { bubbles: true }));
});

// Listen for changes in the system's color scheme preference
window
  .matchMedia("(prefers-color-scheme: dark)")
  .addEventListener("change", (e) => {
    network.setOptions(options());
    updateNetwork({
      nodes: data.nodes.get(),
      edges: data.edges.get(),
    });
  });
