<script lang="ts">
  import Checkbox from "../components/Checkbox.svelte";
  import Input from "../components/Input.svelte";
  import { step, Step } from "../shared.svelte";

  let vertexNameInput = $state("");
  let vertexNameHint = $derived(
    ["Bob", "Alice"].includes(vertexNameInput)
      ? "A vertex with this name already exists"
      : "",
  );

  let vinitInput = $state(true);
</script>

<Input
  id="vertexNameInput"
  heading="Name"
  placeholder="Vertex name"
  hint={vertexNameHint}
  bind:input={vertexNameInput}
/>

<Checkbox id="setVinit" heading="V_init" bind:input={vinitInput} />

<div class="buttons">
  <button
    class="button is-info is-fullwidth"
    onclick={() => step.set(Step.MainMenu)}
  >
    Back
  </button>
</div>
