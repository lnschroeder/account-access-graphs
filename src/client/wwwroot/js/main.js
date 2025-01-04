// utility
/**
 * Sets the disabled state of a button with the given ID. If the button is not
 * yet in the DOM, it observes the DOM for changes and sets the disabled state
 * once the button is available.
 */
function setButtonDisabled(buttonId, isDisabled) {
  const observer = new MutationObserver((mutations, obs) => {
    const button = document.getElementById(buttonId);
    if (button) {
      button.disabled = isDisabled;
      obs.disconnect();
    }
  });

  observer.observe(document.body, {
    childList: true,
    subtree: true,
  });

  const button = document.getElementById(buttonId);
  if (button) {
    button.disabled = isDisabled;
    observer.disconnect();
  }
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
let network = new vis.Network(container, data, options());

const colors = [
  "--bulma-text",
  "--bulma-link",
  "--bulma-primary",
  "--bulma-info",
  "--bulma-success",
  "--bulma-danger",
];

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

  nodes.forEach((node) => {
    let borderWidth = 1;
    let borderColor = getCssVariable("--bulma-link");
    let backgroundColor = getCssVariable("--bulma-text-bold-invert");
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
      label: node.label,
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

    // reorders edges such that the provisional edges come last
    // i.e. they get rendered on top of all other edges
    edges.sort((a, b) => {
      if (a.isProvisional && !b.isProvisional) {
        return 1;
      } else {
        return -1;
      }
    });

    edges.forEach((edge) => {
      let dashes = false;
      let physics = true;
      let color = getCssVariable(colors[edge.index]);
      let width = 1;

      if (edge.isProvisional) {
        dashes = true;
        physics = false;
        color = getCssVariable("--bulma-warning-on-scheme");
        width = 2;
      }

      data.edges.update({
        id: edge.id,
        from: edge.from,
        to: edge.to,
        dashes: dashes,
        physics: physics,
        color: {
          color: color,
        },
        width: width,
      });
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
  let clickedNodeInput = document.getElementById("ClickedNodeInput");

  clickedNodeInput.value = nodeId;
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
