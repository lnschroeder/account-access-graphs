let initNodes = [];
let initEdges = [];

let container = document.getElementById("graph-container");
let data = {
  nodes: new vis.DataSet([]),
  edges: new vis.DataSet([]),
};
let options = { layout: { randomSeed: 2 } };
let network = new vis.Network(container, data, options);

// removes, updates, and adds nodes and edges to the network
function updateNetwork(nodes, edges) {
  console.log(nodes);
  console.log(edges);

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

  data.nodes.remove(removedEdgeIds);
  data.edges.remove(removedNodeIds);
  data.nodes.update(nodes);
  data.edges.update(edges);
}
window.updateNetwork = updateNetwork;

// change focus to text field when clicking button
document.getElementById("addNodeButton").addEventListener("click", () => {
  document.getElementById("nodeName").focus();
});

// click button when pressing enter
let input = document.getElementById("nodeName");
input.addEventListener("keypress", function (event) {
  if (event.key === "Enter") {
    event.preventDefault();
    document.getElementById("addNodeButton").click();
  }
});
