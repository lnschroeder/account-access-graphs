<script lang="ts">
  import {step, Step} from "../../shared.svelte";
  import MainMenu from "../steps/MainMenu.svelte";
  import ModifyVertex from "../steps/ModifyVertex.svelte";
</script>

<div class="box">
  <h1 class="title">{step.value}</h1>
  {#if step.value === Step.MainMenu}
    <MainMenu/>
  {:else if step.value === Step.ModifyVertex}
    <ModifyVertex/>
  {/if}
</div>
