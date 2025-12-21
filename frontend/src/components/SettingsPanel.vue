<script setup lang="ts">
import { useVisNetworkOptionsStore } from '@/stores/visNetworkOptionsStore'

const networkStore = useVisNetworkOptionsStore()
</script>

<template>
  <v-card>
    <v-card-item>
      <v-card-title>Settings</v-card-title>
    </v-card-item>
    <v-card-text>
      <v-switch
        v-model="networkStore.physicsEnabled"
        label="Physics"
        color="primary"
        hide-details
      ></v-switch>
    </v-card-text>
  </v-card>
</template>
