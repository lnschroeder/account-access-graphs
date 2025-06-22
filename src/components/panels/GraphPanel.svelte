<script lang="ts">
  import {onMount} from "svelte";
  import {Network} from "vis-network";
  import {DataSet} from "vis-data";
  import type {Edge, Node} from "../../domain/model";

  let {
    containerId,
    initialNodes,
    initialEdges,
  }: {
    containerId: string;
    initialNodes: DataSet<Node>;
    initialEdges: DataSet<Edge>;
  } = $props();

  let data = {
    nodes: initialNodes,
    edges: initialEdges,
  };

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

  onMount(() => {
    const container = document.getElementById(containerId);
    if (container) {
      new Network(container, data, options());
    } else {
      console.error(
        "Container element with id '" + containerId + "' not found.",
      );
    }
  });
</script>
