// Tipovi prate modele iz backenda (sda/Models).

export interface Project {
  name: string | null
  ecosystem: string | null
  createdAt: string
}

export interface Package {
  name: string | null
  ecosystem: string | null
  description: string | null
}

export interface Vulnerability {
  id: number
  summary: string | null
  severity: string | null
  fixedVersion: string | null
}

// Putanja zavisnosti od projekta do ranjivog paketa:
// ["shop-web", "react-scripts@5.0.1", "...", "minimist@1.2.0"]
export interface VulnerablePath {
  project: string
  vulnerabilityId: number
  path: string[]
}

export interface DashboardData {
  projects: Project[]
  packages: Package[]
  vulnerabilities: Vulnerability[]
  paths: VulnerablePath[]
}
