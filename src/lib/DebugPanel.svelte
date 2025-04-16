<script lang="ts">
  import { step } from "./shared.svelte";
</script>

<div class="box">
  <h1 class="title">Debug info</h1>
  Current step: {step.value}
  <div class="buttons"></div>
</div>
