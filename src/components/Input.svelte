<script lang="ts">
  import Field from "./Field.svelte";

  let {
    id,
    heading,
    placeholder,
    input = $bindable(),
    hint: initialHint,
    required = true,
  }: {
    id: string;
    heading: string;
    placeholder: string;
    input: string;
    hint: string;
    required?: boolean;
  } = $props();
  let hint = $derived(required && !input ? "Field is required" : initialHint);
</script>

{#snippet content()}
  <input
    {id}
    type="text"
    class="input {!hint ? 'text' : 'is-danger'}"
    {placeholder}
    bind:value={input}
  />
{/snippet}

<Field {content} {heading} {hint} {id}/>
