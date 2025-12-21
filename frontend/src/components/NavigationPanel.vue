<script setup lang="ts">
import { useGraphStore } from '@/stores/graphStore'

const graphStore = useGraphStore()
</script>

<template>
  <v-card variant="tonal">
    <v-card-item>
      <v-card-title>Navigation</v-card-title>
    </v-card-item>
    <v-card-text class="d-flex flex-column ga-3">
      <v-btn block prepend-icon="mdi-plus" variant="elevated" @click="graphStore.addNode()">
        Add vertex</v-btn
      >
    </v-card-text>
  </v-card>
</template>
