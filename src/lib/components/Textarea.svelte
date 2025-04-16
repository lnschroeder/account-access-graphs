<script lang="ts">
  import Field from "./Field.svelte";

  let { id, heading, placeholder, validate, required = true } = $props();
  let input = $state("");
  let hint = $derived(
    required && !input ? "Field is required" : validate(input),
  );
</script>

{#snippet content()}
  <textarea
    {id}
    class="textarea {hint === '' ? 'text' : 'is-danger'}"
    {placeholder}
    bind:value={input}
    style="height: 85vh"
  ></textarea>
{/snippet}

<Field {id} {heading} {hint} {input} {content} />
