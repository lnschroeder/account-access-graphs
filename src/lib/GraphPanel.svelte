<script lang="ts">
  import { onMount } from "svelte";
  import { Network } from "vis-network";
  import { DataSet } from "vis-data";

  let { containerId }: { containerId: string } = $props();

  // create an array with nodes
  var nodes = new DataSet([
    { id: 1, label: "Node 1" },
    { id: 2, label: "Node 2" },
    { id: 3, label: "Node 3" },
    { id: 4, label: "Node 4" },
    { id: 5, label: "Node 5" },
  ]);

  // create an array with edges
  var edges = new DataSet([
    { id: 1, from: 1, to: 3 },
    { id: 2, from: 1, to: 2 },
    { id: 3, from: 2, to: 4 },
    { id: 4, from: 2, to: 5 },
    { id: 5, from: 3, to: 3 },
  ]);

  // create a network
  onMount(() => {
    var container = document.getElementById(containerId);
    if (container) {
      var data = {
        nodes: nodes,
        edges: edges,
      };
      var options = {};
      new Network(container, data, options);
    } else {
      console.error(
        "Container element with id '" + containerId + "' not found.",
      );
    }
  });
</script>
