import type { Node } from '@/stores/graphStore'
import { defineStore } from 'pinia'
import { ref, type Ref } from 'vue'

export const useUiStore = defineStore('ui', () => {
  const selectedVertex: Ref<Node | null> = ref(null)

  function selectVertex(vertex: Node) {
    selectedVertex.value = vertex
  }

  return {
    selectedVertex,
    selectVertex,
  }
})
