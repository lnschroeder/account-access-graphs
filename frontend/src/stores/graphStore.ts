import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface Node {
  id: number
  label: string
}

export interface Edge {
  from: number
  to: number
  label: string
}

export interface GraphData {
  nodes: Node[]
  edges: Edge[]
}

export const useGraphStore = defineStore('graph', () => {
  const nodes = ref<Node[]>([
    { id: 1, label: 'Alice' },
    { id: 2, label: 'Bob' },
    { id: 3, label: 'Charlie' },
  ])

  const edges = ref<Edge[]>([
    { from: 1, to: 2, label: 'knows' },
    { from: 2, to: 3, label: 'owes' },
  ])

  const graphData = computed<GraphData>(() => ({
    nodes: nodes.value,
    edges: edges.value,
  }))

  function addNode() {
    const id = nodes.value.length + 1
    nodes.value.push({ id, label: `Node ${id}` })
  }

  return {
    nodes,
    edges,
    graphData,
    addNode,
  }
})
