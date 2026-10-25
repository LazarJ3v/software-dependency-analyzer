// Vite izlaze samo promenljive sa prefiksom VITE_.
// VITE_USE_REAL_DATA=true  -> pravi podaci iz backend API-ja (Neo4j)
// VITE_USE_REAL_DATA=false -> mock podaci (backend nije potreban)
export const USE_REAL_DATA: boolean =
  String(import.meta.env.VITE_USE_REAL_DATA ?? 'false').toLowerCase() === 'true'

export const API_URL: string = (
  import.meta.env.VITE_API_URL ?? 'http://localhost:5027'
).replace(/\/+$/, '')
