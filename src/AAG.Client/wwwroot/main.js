function renderGraph(graphData = "dinetwork { }") {
  let parsedData = vis.parseDOTNetwork(graphData);
  let data = {
    nodes: parsedData.nodes,
    edges: parsedData.edges,
  };

  let options = parsedData.options;
  options.layout = {
    randomSeed: 2,
  };
  options.nodes = {
    color: "red",
  };
  let container = document.getElementById("graph-container");

  new vis.Network(container, data, options);
}
window.renderGraph = renderGraph;

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
