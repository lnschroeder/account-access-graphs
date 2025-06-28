<script lang="ts">
  import {onMount} from "svelte";
  import type {Graph} from "../../domain/model";
  import {getNetwork} from "../../transformer/visjs";

  let {
    containerId,
    graph,
  }: {
    containerId: string;
    graph: Graph
  } = $props();

  onMount(() => {
    const container = document.getElementById(containerId);
    if (container) {
      getNetwork(container, graph)
    } else {
      console.error(
        "Container element with id '" + containerId + "' not found.",
      );
    }
  });
</script>
