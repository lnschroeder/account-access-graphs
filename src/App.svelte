<script lang="ts">
  import ControlPanel from "./lib/ControlPanel.svelte";
  import GraphPanel from "./lib/GraphPanel.svelte";
  import DebugPanel from "./lib/DebugPanel.svelte";
  import JsonPanel from "./lib/JsonPanel.svelte";
</script>

<main>
  <div class="columns">
    <div class="column is-one-third">
      <ControlPanel />
      <DebugPanel />
    </div>
    <div class="column" style="display: flex">
      <div class="box p-0" style="flex:1; min-height: 85vh">
        <GraphPanel />
      </div>
    </div>
  </div>

  <div class="box">
    <JsonPanel />
  </div>
</main>
