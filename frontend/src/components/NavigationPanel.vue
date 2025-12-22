<script setup lang="ts">
import MainMenu from './navigationPanel/MainMenu.vue'
import VertexMenu from './navigationPanel/VertexMenu.vue'
import { useUiStore } from '@/stores/uiStore'
import { computed } from 'vue'

type ViewComponent = {
  component: typeof MainMenu | typeof VertexMenu
  title: string
}

const uiStore = useUiStore()
const currentView = computed<ViewComponent>(() => {
  if (uiStore.selectedVertex)
    return {
      component: VertexMenu,
      title: `Vertex Menu ${JSON.stringify(uiStore.selectedVertex)}`,
    }
  return {
    component: MainMenu,
    title: 'Main Menu',
  }
})
</script>

<template>
  <v-card>
    <v-card-item>
      <v-card-title>{{ currentView.title }}</v-card-title>
    </v-card-item>

    <component :is="currentView.component" />
  </v-card>
</template>
