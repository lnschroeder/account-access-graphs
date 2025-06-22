<script lang="ts">
  import ControlPanel from "./components/panels/ControlPanel.svelte";
  import GraphPanel from "./components/panels/GraphPanel.svelte";
  import DebugPanel from "./components/panels/DebugPanel.svelte";
  import JsonPanel from "./components/panels/JsonPanel.svelte";
  import {DataSet} from "vis-data";
  import type {Edge, Node} from "./domain/model";

  let graphContainerId = "aagNetwork";

  const nodes = new DataSet<Node>([
    {id: 1, label: "Node 1"},
    {id: 2, label: "Node 2"},
    {id: 3, label: "Node 3"},
    {id: 4, label: "Node 4"},
    {id: 5, label: "Node 5"},
  ]);

  const edges = new DataSet<Edge>([
    {id: 1, from: 1, to: 2},
    {id: 2, from: 1, to: 2},
    {id: 3, from: 2, to: 4},
    {id: 4, from: 2, to: 5},
    {id: 5, from: 3, to: 3},
  ]);

  const abc = () => {
    // nodes.update({ id: 7, label: "huhu 7" });
    // nodes.updateOnly({ id: 8, label: "huhu 8" });
  };
</script>

<main>
  <div class="columns">
    <div class="column is-one-third">
      <ControlPanel/>
      <DebugPanel/>
      <button class="button is-info is-fullwidth" onclick={() => abc()}>
        Add
      </button>
    </div>
    <div class="column" style="display: flex">
      <div class="box p-0" id={graphContainerId} style="flex:1; height: 85vh">
        <GraphPanel
          containerId={graphContainerId}
          initialEdges={edges}
          initialNodes={nodes}
        />
      </div>
    </div>
  </div>

  <div class="box">
    <JsonPanel/>
  </div>
</main>
