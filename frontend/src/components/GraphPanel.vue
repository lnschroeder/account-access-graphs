<script setup lang="ts">
import { useGraphStore, type Node } from '@/stores/graphStore'
import { useVisNetworkOptionsStore } from '@/stores/visNetworkOptionsStore'
import { DataSet } from 'vis-data'
import {
  Network,
  type Data,
  type DataSetNodes,
  type DataSetEdges,
  type Position,
} from 'vis-network'
import { ref, onMounted, onUnmounted, watch } from 'vue'

const graphContainer = ref<HTMLElement | null>(null)

const graphStore = useGraphStore()
const networkStore = useVisNetworkOptionsStore()
const nodes: DataSetNodes = new DataSet(graphStore.graphData.nodes)
const edges: DataSetEdges = new DataSet(graphStore.graphData.edges)
const data: Data = { nodes, edges }

let network: Network | null = null

function getDiff(): { addedNodes: Node[] } {
  const oldNodeIds = nodes.getIds()
  const addedNodes: Node[] = graphStore.graphData.nodes.filter(
    (node) => !oldNodeIds.includes(node.id),
  )

  return { addedNodes }
}

function getNextNodePosition(): Position {
  const lastNodeId = nodes.get()[nodes.length - 1]?.id

  if (!network || !lastNodeId) {
    return { x: 0, y: 0 }
  }

  const lastNodePosition = network.getPosition(lastNodeId)

  return {
    x: lastNodePosition.x + 20,
    y: lastNodePosition.y + 10,
  }
}

watch(
  () => graphStore.graphData,
  () => {
    const { addedNodes } = getDiff()

    addedNodes.forEach((node) => {
      const position = getNextNodePosition()
      nodes.add({
        ...node,
        x: position.x,
        y: position.y,
      })
    })
  },
  { deep: true },
)

watch(
  () => networkStore.options,
  (newOptions) => network?.setOptions(newOptions),
)

onMounted(async () => {
  // initialize the network only after fonts are loaded
  try {
    await document.fonts.ready
  } catch (error) {
    console.error('Font loading failed', error)
  }

  if (graphContainer.value) {
    network = new Network(graphContainer.value, data, networkStore.options)
  }
})

onUnmounted(() => {
  network?.destroy()
  network = null
})
</script>

<template>
  <v-card class="overflow-hidden" height="80vh">
    <div class="fill-height" ref="graphContainer"></div>
  </v-card>
</template>
