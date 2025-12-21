<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue'
import { useTheme } from 'vuetify'
import { Network } from 'vis-network'
import type { Data, Options } from 'vis-network'

const theme = useTheme()
const graphContainer = ref<HTMLElement | null>(null)
let network: Network | null = null

const data: Data = {
  nodes: [
    { id: 1, label: 'Alice' },
    { id: 2, label: 'Bob' },
    { id: 3, label: 'Charlie' },
  ],
  edges: [
    { from: 1, to: 2, label: 'knows' },
    { from: 2, to: 3, label: 'owes' },
  ],
}

const getGraphOptions = (): Options => {
  const colors = theme.current.value.colors
  return {
    layout: {
      randomSeed: 0,
    },
    nodes: {
      shape: 'box',
      chosen: false,
      color: {
        background: colors['surface'],
        border: colors['surface-variant'],
      },
      font: {
        color: colors['surface-variant'],
      },
    },
    edges: {
      color: colors['surface-variant'],
      arrows: { to: { enabled: true } },
      chosen: false,
      font: {
        color: colors['surface-variant'],
        background: colors['surface'],
        strokeWidth: 0,
      },
    },
    physics: { enabled: true },
  }
}

onMounted(() => {
  if (graphContainer.value) {
    network = new Network(graphContainer.value, data, getGraphOptions())
  }
})

watch(theme.current, () => {
  network?.setOptions(getGraphOptions())
})

onUnmounted(() => {
  network?.destroy()
})
</script>

<template>
  <v-card height="80vh" variant="elevated" color="surface">
    <div ref="graphContainer" class="h-100 w-100"></div>
  </v-card>
</template>
