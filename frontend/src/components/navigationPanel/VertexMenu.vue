<script setup lang="ts">
import { useUiStore } from '@/stores/uiStore'

const uiStore = useUiStore()
</script>

<template>
  <v-card-text class="d-flex flex-column ga-3">
    <v-btn block prepend-icon="mdi-arrow-left" color="primary" @click="uiStore.selectedVertex = null">
      Back to main menu</v-btn
    >
  </v-card-text>
</template>
