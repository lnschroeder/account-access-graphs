<script lang="ts">
  import Textarea from "../Textarea.svelte";

  let aagJsonInput = $state("");
  let aagJsonHint = "";
</script>

<Textarea
  bind:input={aagJsonInput}
  heading="JSON representation"
  hint={aagJsonHint}
  id="jsonInput"
  placeholder="JSON representation"
/>
