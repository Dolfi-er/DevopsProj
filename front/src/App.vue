<template>
  <div class="app">
    <h1>Weather Forecast</h1>
    
    <button @click="fetchWeather" :disabled="loading" class="refresh-btn">
      {{ loading ? 'Loading...' : 'Get Weather' }}
    </button>

    <div v-if="error" class="error">{{ error }}</div>

    <div class="weather-list" v-else-if="forecasts.length">
      <div v-for="forecast in forecasts" :key="forecast.date" class="weather-item">
        <div class="date">{{ formatDate(forecast.date) }}</div>
        <div class="temp">{{ forecast.temperatureC }}°C / {{ forecast.temperatureF }}°F</div>
        <div class="summary">{{ forecast.summary }}</div>
      </div>
    </div>
    
    <div v-else-if="!loading" class="no-data">
      No weather data available. Click "Get Weather" to load data.
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      forecasts: [],
      loading: false,
      error: null
    }
  },
  methods: {
    async fetchWeather() {
      this.loading = true
      this.error = null
      try {
        // Используем прокси через /api
        const response = await fetch('/api/weatherforecast')
        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`)
        this.forecasts = await response.json()
      } catch (err) {
        this.error = `Failed to load weather data: ${err.message}`
        console.error('Error:', err)
      } finally {
        this.loading = false
      }
    },
    formatDate(dateString) {
      return new Date(dateString).toLocaleDateString()
    }
  }
  // Убрали mounted, чтобы данные не загружались автоматически
}
</script>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  background: #f5f5f5;
  color: #333;
}

.app {
  max-width: 600px;
  margin: 0 auto;
  padding: 2rem;
}

h1 {
  text-align: center;
  margin-bottom: 2rem;
  color: #2c3e50;
}

.refresh-btn {
  display: block;
  width: 100%;
  padding: 12px;
  background: #3498db;
  color: white;
  border: none;
  border-radius: 6px;
  font-size: 16px;
  cursor: pointer;
  margin-bottom: 2rem;
}

.refresh-btn:disabled {
  background: #bdc3c7;
  cursor: not-allowed;
}

.refresh-btn:hover:not(:disabled) {
  background: #2980b9;
}

.weather-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.weather-item {
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
  border-left: 4px solid #3498db;
}

.date {
  font-weight: bold;
  font-size: 1.1rem;
  margin-bottom: 0.5rem;
  color: #2c3e50;
}

.temp {
  color: #e74c3c;
  font-weight: 500;
  margin-bottom: 0.5rem;
}

.summary {
  color: #7f8c8d;
  font-style: italic;
}

.error {
  background: #e74c3c;
  color: white;
  padding: 1rem;
  border-radius: 6px;
  text-align: center;
  margin-bottom: 1rem;
}

.no-data {
  text-align: center;
  color: #7f8c8d;
  font-style: italic;
  padding: 2rem;
}

@media (max-width: 600px) {
  .app {
    padding: 1rem;
  }
  
  .weather-item {
    padding: 1rem;
  }
}
</style>