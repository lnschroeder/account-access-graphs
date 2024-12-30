// change focus to text field when clicking button
document.getElementById("addVertexButton").addEventListener("click", () => {
  document.getElementById("vertexName").focus();
});

// click button when pressing enter
let input = document.getElementById("vertexName");
input.addEventListener("keypress", function (event) {
  if (event.key === "Enter") {
    event.preventDefault();
    document.getElementById("addVertexButton").click();
  }
});
