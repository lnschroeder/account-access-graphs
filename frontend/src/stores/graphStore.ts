import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export interface Node {
  id: string
  label: string
}

export interface Edge {
  id: string
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
    { id: '1', label: 'Alice' },
    { id: '2', label: 'Bob' },
    { id: '3', label: 'Charlie' },
  ])

  const edges = ref<Edge[]>([
    { id: '1', from: 1, to: 2, label: 'knows' },
    { id: '2', from: 2, to: 3, label: 'owes' },
  ])

  const graphData = computed<GraphData>(() => ({
    nodes: nodes.value,
    edges: edges.value,
  }))

  function addNode(): Node {
    const id = nodes.value.length + 1
    const node = { id: `${id}`, label: `Node ${id}` }

    nodes.value.push(node)

    return node
  }

  return {
    nodes,
    edges,
    graphData,
    addNode,
  }
})
