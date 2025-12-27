import { defineStore } from 'pinia'
import type { Options, Font } from 'vis-network'
import { ref, computed } from 'vue'
import { useTheme } from 'vuetify'

export const useVisNetworkOptionsStore = defineStore('visNetworkOptions', () => {
  const theme = useTheme()
  const physicsEnabled = ref(true)

  const fontFace = 'Roboto, Arial'

  const options = computed<Options>(() => {
    const colors = theme.current.value.colors
    const surfaceColor = colors['surface']
    const onSurfaceColor = colors['on-surface']

    const font: Font = {
      face: fontFace,
      color: onSurfaceColor,
      background: surfaceColor,
      strokeWidth: 0,
    }

    return {
      layout: { randomSeed: 0 },

      physics: { enabled: physicsEnabled.value },

      nodes: {
        shape: 'box',
        shapeProperties: { borderRadius: 4 },
        chosen: false,
        color: {
          background: surfaceColor,
          border: onSurfaceColor,
        },
        font: font,
      },

      edges: {
        color: onSurfaceColor,
        arrows: { to: { enabled: true } },
        chosen: false,
        font: font,
      },
    }
  })

  return {
    options,
    physicsEnabled,
  }
})
