// Plugins
import { registerPlugins } from '@/plugins'

// Composables
import { createApp } from 'vue'

// Components
import App from './App.vue'

// Styles
import 'unfonts.css'

const app = createApp(App)

registerPlugins(app)

app.mount('#app')
