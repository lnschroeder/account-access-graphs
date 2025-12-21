import App from '@/App.vue'
import { registerPlugins } from '@/plugins'
import 'unfonts.css'
import { createApp } from 'vue'

const app = createApp(App)

registerPlugins(app)

app.mount('#app')
