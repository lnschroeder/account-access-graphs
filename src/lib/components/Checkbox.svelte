<script lang="ts">
  import Field from "./Field.svelte";

  let {
    id,
    heading,
    input = $bindable(),
    hint = "",
    defaultValue = false,
  }: {
    id: string;
    heading: string;
    input: boolean;
    hint?: string;
    defaultValue?: boolean;
  } = $props();
</script>

{#snippet content()}
  <label class="checkbox">
    <input {id} type="checkbox" bind:checked={input} {defaultValue} />
  </label>
{/snippet}

<Field {id} {heading} {hint} {content} />
