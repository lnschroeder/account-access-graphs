<script lang="ts">
  let { id, heading, content, input, hint } = $props();
</script>

<div class="field">
  <!-- Label -->
  <label class="label" for={id}>{heading}</label>

  <!-- Control -->
  <div class="control">
    {@render content(input)}
  </div>

  <!-- Hint -->
  <!-- {#if hint !== ""} -->
  <p class="help is-danger">{hint}</p>
</div>
