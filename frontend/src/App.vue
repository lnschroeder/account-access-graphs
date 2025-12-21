<script setup lang="ts">
import NavigationPanel from '@/components/NavigationPanel.vue'
import InfoPanel from '@/components/SettingsPanel.vue'
import GraphPanel from '@/components/GraphPanel.vue'
import JsonPanel from '@/components/JsonPanel.vue'
import AppBar from '@/components/AppBar.vue'
</script>

<template>
  <v-app>
    <AppBar />
    
    <v-main>
      <v-container fluid>
        <v-row>
          <!-- Left Column -->
          <v-col cols="12" md="4">
            <v-row>
              <v-col cols="12">
                <NavigationPanel />
              </v-col>

              <v-col cols="12">
                <InfoPanel />
              </v-col>
            </v-row>
          </v-col>

          <!-- Right Column -->
          <v-col cols="12" md="8">
            <GraphPanel />
          </v-col>
        </v-row>

        <!-- Footer Row -->
        <v-row>
          <v-col cols="12">
            <JsonPanel />
          </v-col>
        </v-row>
      </v-container>
    </v-main>
  </v-app>
</template>
