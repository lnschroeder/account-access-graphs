<script setup lang="ts">
import { useGraphStore } from '@/stores/graphStore'
import { useUiStore } from '@/stores/uiStore'

const graphStore = useGraphStore()

function addVertexAndSwitchView() {
  const newNode = graphStore.addNode()
  useUiStore().selectVertex(newNode)
}
</script>

<template>
  <v-card-text class="d-flex flex-column ga-3">
    <v-btn block prepend-icon="mdi-plus" @click="addVertexAndSwitchView()" color="primary">
      Add vertex</v-btn
    >
  </v-card-text>
</template>
