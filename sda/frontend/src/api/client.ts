import { API_URL, USE_REAL_DATA } from '../config'
import { mockData } from '../data/mock'
import type {
  DashboardData,
  Package,
  Project,
  Vulnerability,
  VulnerablePath,
} from '../types'

async function get<T>(path: string): Promise<T> {
  const url = `${API_URL}${path}`
  let res: Response
  try {
    res = await fetch(url)
  } catch {
    throw new Error(`Backend nije dostupan (${url}). Da li je API pokrenut?`)
  }
  if (!res.ok) {
    throw new Error(`Zahtev ${url} nije uspeo (HTTP ${res.status}).`)
  }
  return (await res.json()) as T
}

async function loadMock(): Promise<DashboardData> {
  // Mala pauza da se vidi stanje ucitavanja.
  await new Promise((resolve) => setTimeout(resolve, 300))
  return mockData
}

async function loadReal(): Promise<DashboardData> {
  // NAPOMENA: ovi GET endpointi moraju postojati u backendu.
  // Trenutno ApiController ima samo POST endpoint-e.
  const [projects, packages, vulnerabilities, paths] = await Promise.all([
    get<Project[]>('/api/project'),
    get<Package[]>('/api/package'),
    get<Vulnerability[]>('/api/vulnerability'),
    get<VulnerablePath[]>('/api/analysis/vulnerable-paths'),
  ])
  return { projects, packages, vulnerabilities, paths }
}

export function loadDashboardData(): Promise<DashboardData> {
  return USE_REAL_DATA ? loadReal() : loadMock()
}
