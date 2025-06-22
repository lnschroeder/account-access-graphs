<script lang="ts">
  import {step, Step} from "../../shared.svelte";
</script>

<div class="buttons">
  <button
    class="button is-info is-fullwidth"
    onclick={() => step.set(Step.ModifyVertex)}
  >
    Add vertex
  </button>
</div>
