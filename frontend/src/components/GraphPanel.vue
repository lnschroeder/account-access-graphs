<script setup lang="ts">
import { ref, onMounted, onUnmounted, watch } from 'vue'
import { Network } from 'vis-network'
import { useGraphStore } from '@/stores/graphStore'
import { useVisNetworkOptionsStore } from '@/stores/visNetworkOptionsStore'

const graphContainer = ref<HTMLElement | null>(null)

const graphStore = useGraphStore()
const networkStore = useVisNetworkOptionsStore()

let network: Network | null = null

watch(
  () => graphStore.graphData,
  (newData) => network?.setData(newData),
  { deep: true },
)

watch(
  () => networkStore.options,
  (newOptions) => network?.setOptions(newOptions),
)

onMounted(() => {
  if (graphContainer.value) {
    network = new Network(graphContainer.value, graphStore.graphData, networkStore.options)
  }
})

onUnmounted(() => {
  network?.destroy()
  network = null
})
</script>

<template>
  <v-card height="80vh" class="overflow-hidden">
    <div ref="graphContainer" class="fill-height"></div>
  </v-card>
</template>
