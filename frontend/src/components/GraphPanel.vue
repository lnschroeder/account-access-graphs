<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue'
import { useTheme } from 'vuetify'
import { Network } from 'vis-network'
import type { Options } from 'vis-network'
import { useGraphStore } from '@/stores/graphStore'

const theme = useTheme()
const graphContainer = ref<HTMLElement | null>(null)
let network: Network | null = null

const graphStore = useGraphStore()

watch(
  () => graphStore.graphData,
  (newData) => {
    network?.setData(newData)
  },
  { deep: true },
)

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
    network = new Network(graphContainer.value, graphStore.graphData, getGraphOptions())
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
