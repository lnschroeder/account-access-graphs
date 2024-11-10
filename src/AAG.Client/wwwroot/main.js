function renderGraph(graphData = "dinetwork { }") {
  var parsedData = vis.parseDOTNetwork(graphData);

  var data = {
    nodes: parsedData.nodes,
    edges: parsedData.edges,
  };

  var options = parsedData.options;
  options.layout = {
    randomSeed: 2,
  };
  options.nodes = {
    color: "red",
  };
  var container = document.getElementById("graph-container");

  // create a network
  var network = new vis.Network(container, data, options);
  sleep(2);
  data.nodes;
}

// change focus to text field when clicking button
document.getElementById("addNodeButton").addEventListener("click", () => {
  document.getElementById("nodeName").focus();
});

// click button when pressing enter
// Get the input field
var input = document.getElementById("nodeName");

// Execute a function when the user presses a key on the keyboard
input.addEventListener("keypress", function (event) {
  // If the user presses the "Enter" key on the keyboard
  if (event.key === "Enter") {
    // Cancel the default action, if needed
    event.preventDefault();
    // Trigger the button element with a click
    document.getElementById("addNodeButton").click();
  }
});
