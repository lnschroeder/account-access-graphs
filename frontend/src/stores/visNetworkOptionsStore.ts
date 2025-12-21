import { defineStore } from 'pinia'
import type { Options } from 'vis-network'
import { ref, computed } from 'vue'
import { useTheme } from 'vuetify'

export const useVisNetworkOptionsStore = defineStore('visNetworkOptions', () => {
  const theme = useTheme()
  const physicsEnabled = ref(true)
  const options = computed<Options>(() => {
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
        arrows: {
          to: {
            enabled: true,
          },
        },
        chosen: false,
        font: {
          color: colors['surface-variant'],
          background: colors['surface'],
          strokeWidth: 0,
        },
      },
      physics: { enabled: physicsEnabled.value },
    }
  })

  return {
    options,
    physicsEnabled,
  }
})
