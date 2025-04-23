<script lang="ts">
  import Field from "./Field.svelte";
  import '@fortawesome/fontawesome-free/css/all.min.css'

  let {
    id,
    heading,
    headers,
    body,
    hint = "",
  }: {
    id: string;
    heading: string;
    headers: string[];
    body: any;
    hint?: string;
  } = $props();
</script>

{#snippet content()}
  <table class="table is-hoverable is-narrow">
    <thead>
      <tr>
        {#each headers as header}
          <th>{header}</th>
        {/each}
      </tr>
    </thead>
    <tbody>
      {@render body()}
    </tbody>
  </table>
{/snippet}

<Field {id} {heading} {hint} {content} />
