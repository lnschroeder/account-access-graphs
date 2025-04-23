<script lang="ts">
  import Checkbox from "../Checkbox.svelte";
  import Input from "../Input.svelte";
  import Table from "../Table.svelte";
  import { step, Step } from "../../shared.svelte";

  let vertexNameInput = $state("");
  let vertexNameHint = $derived(
    ["Bob", "Alice"].includes(vertexNameInput)
      ? "A vertex with this name already exists"
      : "",
  );

  let vinitInput = $state(true);
  let accessMethods = [
    { name: "access 1", factors: [{ name: "factor 1" }, { name: "factor 2" }] },
    { name: "access 2", factors: [{ name: "factor 1" }, { name: "factor 3" }] },
  ];
</script>

<Input
  id="vertexNameInput"
  heading="Name"
  placeholder="Vertex name"
  hint={vertexNameHint}
  bind:input={vertexNameInput}
/>

<Checkbox id="setVinit" heading="V_init" bind:input={vinitInput} />

{#snippet accessMethodsBody()}
  {#each accessMethods as accessMethod}
    <tr>
      <td>{accessMethod.name}</td>
      <td>
        <div class="tags">
          {#each accessMethod.factors as factor}
            <span class="tag">{factor.name}</span>
          {/each}
        </div>
      </td>
      <td>
        <button class="tag is-info" aria-label="edit access">
          <span class="icon is-small">
            <i class="fas fa-edit"></i>
          </span>
        </button>
      </td>
    </tr>
  {/each}
  <tr>
    <td></td>
    <td></td>
    <td>
      <button class="tag is-success" aria-label="add access">
        <span class="icon is-small">
          <i class="fas fa-plus"></i>
        </span>
      </button>
    </td>
  </tr>
{/snippet}

<Table
  id="accesses"
  heading="Access methods"
  headers={["a", "b", ""]}
  body={accessMethodsBody}
/>

<div class="buttons">
  <button
    class="button is-info is-fullwidth"
    onclick={() => step.set(Step.MainMenu)}
  >
    Back
  </button>
</div>
