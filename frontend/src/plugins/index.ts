import vuetify from '@/plugins/vuetify'
import pinia from '@/stores'
import type { App } from 'vue'

export function registerPlugins(app: App) {
  app.use(vuetify).use(pinia)
}
