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
let network = new vis.Network(container, data, options());

// "Vibrant qualitative color-blind safe color scheme" by Paul Tol
// source: https://personal.sron.nl/~pault/#sec:qualitative
const colors = [
  "#EE7733",
  "#0077BB",
  "#EE3377",
  "#009988",
  "#CC3311",
  "#33BBEE",
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
    let backgroundColor = window.matchMedia("(prefers-color-scheme: dark)")
      .matches
      ? "#14161A"
      : "#FFFFFF";
    let fontColor = getCssVariable("--bulma-text-bold");
    let borderDashes = false;
    let borderColor = fontColor;
    let label = node.label + "\n" + node.score;

    if (node.isSubject) {
      borderColor = getCssVariable("--bulma-warning-on-scheme");
      borderWidth = 3;
    }

    if (node.isFactor) {
      borderColor = getCssVariable("--bulma-warning-on-scheme");
      borderWidth = 3;
      borderDashes = [5, 5];
    }

    if (node.isInitiallyCompromised) {
      borderColor = getCssVariable("--bulma-danger-on-scheme");
      borderWidth = 3;
    }

    if (node.isTransitivelyCompromised) {
      borderColor = getCssVariable("--bulma-danger-on-scheme");
      borderWidth = 3;
      borderDashes = [5, 5];
    }

    if (node.isVinit) {
      backgroundColor = getCssVariable("--bulma-success");
      fontColor = getCssVariable("--bulma-text-bold-invert");
    }

    data.nodes.update({
      id: node.id,
      label: label,
      borderWidth: borderWidth,
      color: {
        border: borderColor,
        background: backgroundColor,

        highlight: {
          border: borderColor,
          background: backgroundColor,
        },
      },
      shapeProperties: {
        borderDashes: borderDashes,
      },

      font: {
        color: fontColor,
      },
    });

    edges.forEach((edge) => {
      let dashes = false;
      let physics = true;
      let color = colors[edge.colorIndex % colors.length]; // TODO use random color instead
      let width = 1;

      if (edge.isProvisional) {
        dashes = true;
        width = 2;
      }

      if (edge.isHighlighted) {
        width = 3;
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
  let edgeId = this.getEdgeAt(params.pointer.DOM);

  let clickedVertexInput = document.getElementById("ClickedVertexInput");
  let clickedEdgeInput = document.getElementById("ClickedEdgeInput");
  let clickedBackgroundButton = document.getElementById(
    "ClickedBackgroundButton"
  );

  if (nodeId == null && edgeId == null) {
    clickedBackgroundButton.dispatchEvent(
      new Event("click", { bubbles: true })
    );
  } else if (nodeId != null) {
    clickedVertexInput.value = nodeId;
    clickedVertexInput.dispatchEvent(new Event("input", { bubbles: true }));
  } else {
    clickedEdgeInput.value = edgeId;
    clickedEdgeInput.dispatchEvent(new Event("input", { bubbles: true }));
  }
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
