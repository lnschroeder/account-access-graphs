<script lang="ts">
  import Textarea from "./components/Textarea.svelte";

  let aagJsonInput = $state("");
  let aagJsonHint = "";
</script>

<Textarea
  id="jsonInput"
  heading="JSON representation"
  placeholder="JSON representation"
  hint={aagJsonHint}
  bind:input={aagJsonInput}
/>
