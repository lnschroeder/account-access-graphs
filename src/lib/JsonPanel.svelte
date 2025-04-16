<script lang="ts">
  import Textarea from "./components/Textarea.svelte";

  function validateJson(input: string): string {
    return "";
  }
</script>

<Textarea
  heading="JSON representation"
  id="jsonInput"
  placeholder="JSON representation"
  validate={validateJson}
/>
