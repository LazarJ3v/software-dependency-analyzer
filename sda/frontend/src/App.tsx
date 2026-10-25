import { useEffect, useMemo, useState } from 'react'
import { loadDashboardData } from './api/client'
import { USE_REAL_DATA } from './config'
import ProjectModal from './ProjectModal'
import type { DashboardData, Project, Vulnerability } from './types'

type Severity = 'critical' | 'high' | 'medium' | 'low' | 'unknown'

function normalizeSeverity(value: string | null): Severity {
  const v = (value ?? '').toLowerCase()
  return v === 'critical' || v === 'high' || v === 'medium' || v === 'low' ? v : 'unknown'
}

const SEVERITY_ORDER: Severity[] = ['critical', 'high', 'medium', 'low', 'unknown']

function SeverityBadge({ value }: { value: string | null }) {
  const s = normalizeSeverity(value)
  return <span className={`badge badge-${s}`}>{s}</span>
}

function StatCard({ label, value, tone }: { label: string; value: number; tone?: string }) {
  return (
    <div className={`stat ${tone ? `stat-${tone}` : ''}`}>
      <span className="stat-value">{value}</span>
      <span className="stat-label">{label}</span>
    </div>
  )
}

function formatDate(iso: string): string {
  const d = new Date(iso)
  return Number.isNaN(d.getTime()) ? '-' : d.toLocaleDateString('sr-RS')
}

function PathView({ path }: { path: string[] }) {
  return (
    <ol className="path">
      {path.map((node, i) => (
        <li key={`${node}-${i}`} className={i === path.length - 1 ? 'path-end' : ''}>
          {node}
        </li>
      ))}
    </ol>
  )
}

function Dashboard({ data }: { data: DashboardData }) {
  const [selectedProject, setSelectedProject] = useState<string | null>(null)
  // Projekti kreirani u ovoj sesiji; cuvaju se samo u frontendu (nista se ne salje backendu).
  const [createdProjects, setCreatedProjects] = useState<Project[]>([])
  const [modalOpen, setModalOpen] = useState(false)
  const projects = useMemo(
    () => [...createdProjects, ...data.projects],
    [createdProjects, data.projects],
  )

  const vulnById = useMemo(() => {
    const map = new Map<number, Vulnerability>()
    data.vulnerabilities.forEach((v) => map.set(v.id, v))
    return map
  }, [data.vulnerabilities])

  const affectedProjects = useMemo(
    () => new Set(data.paths.map((p) => p.project)),
    [data.paths],
  )

  const critical = data.vulnerabilities.filter(
    (v) => normalizeSeverity(v.severity) === 'critical',
  ).length

  const visiblePaths = useMemo(
    () =>
      data.paths
        .filter((p) => !selectedProject || p.project === selectedProject)
        .sort((a, b) => {
          const sa = normalizeSeverity(vulnById.get(a.vulnerabilityId)?.severity ?? null)
          const sb = normalizeSeverity(vulnById.get(b.vulnerabilityId)?.severity ?? null)
          return SEVERITY_ORDER.indexOf(sa) - SEVERITY_ORDER.indexOf(sb)
        }),
    [data.paths, selectedProject, vulnById],
  )

  const sortedVulns = [...data.vulnerabilities].sort(
    (a, b) =>
      SEVERITY_ORDER.indexOf(normalizeSeverity(a.severity)) -
      SEVERITY_ORDER.indexOf(normalizeSeverity(b.severity)),
  )

  return (
    <>
      <section className="stats">
        <StatCard label="Projekti" value={projects.length} />
        <StatCard label="Paketi" value={data.packages.length} />
        <StatCard label="Ranjivosti" value={data.vulnerabilities.length} tone="warn" />
        <StatCard label="Kriticne" value={critical} tone="danger" />
        <StatCard label="Pogodjeni projekti" value={affectedProjects.size} tone="danger" />
      </section>

      <div className="grid">
        <section className="card">
          <div className="card-head">
            <h2>Projekti</h2>
            <button className="refresh small" onClick={() => setModalOpen(true)}>
              + Novi projekat
            </button>
          </div>
          {projects.length === 0 ? (
            <p className="empty">Nema projekata.</p>
          ) : (
            <ul className="project-list">
              {projects.map((p) => {
                const name = p.name ?? '(bez imena)'
                const affected = affectedProjects.has(name)
                const active = selectedProject === name
                return (
                  <li key={name}>
                    <button
                      className={`project ${active ? 'active' : ''}`}
                      onClick={() => setSelectedProject(active ? null : name)}
                      aria-pressed={active}
                    >
                      <span className="project-name">{name}</span>
                      <span className="project-meta">
                        {p.ecosystem ?? '-'} · {formatDate(p.createdAt)}
                        {p.dependencyCount !== undefined && ` · ${p.dependencyCount} zavisnosti`}
                      </span>
                      {p.lockFileName ? (
                        <span className="dot dot-new">nije analiziran</span>
                      ) : (
                        <span className={`dot ${affected ? 'dot-bad' : 'dot-ok'}`}>
                          {affected ? 'ranjiv' : 'cist'}
                        </span>
                      )}
                    </button>
                  </li>
                )
              })}
            </ul>
          )}
        </section>

        <section className="card card-wide">
          <h2>
            Putanje do ranjivih paketa
            {selectedProject && <span className="filter"> · {selectedProject}</span>}
          </h2>
          {visiblePaths.length === 0 ? (
            <p className="empty">Nema pronadjenih ranjivih putanja.</p>
          ) : (
            <ul className="findings">
              {visiblePaths.map((p, i) => {
                const v = vulnById.get(p.vulnerabilityId)
                return (
                  <li key={`${p.project}-${p.vulnerabilityId}-${i}`} className="finding">
                    <div className="finding-head">
                      <SeverityBadge value={v?.severity ?? null} />
                      <span className="finding-summary">
                        {v?.summary ?? `Ranjivost #${p.vulnerabilityId}`}
                      </span>
                      {v?.fixedVersion && (
                        <span className="fix">popravka u {v.fixedVersion}</span>
                      )}
                    </div>
                    <PathView path={p.path} />
                  </li>
                )
              })}
            </ul>
          )}
        </section>

        <section className="card">
          <h2>Poznate ranjivosti</h2>
          {sortedVulns.length === 0 ? (
            <p className="empty">Nema ranjivosti.</p>
          ) : (
            <table>
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Ozbiljnost</th>
                  <th>Opis</th>
                  <th>Popravka</th>
                </tr>
              </thead>
              <tbody>
                {sortedVulns.map((v) => (
                  <tr key={v.id}>
                    <td className="mono">#{v.id}</td>
                    <td>
                      <SeverityBadge value={v.severity} />
                    </td>
                    <td>{v.summary ?? '-'}</td>
                    <td className="mono">{v.fixedVersion ?? '-'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </section>

        <section className="card">
          <h2>Paketi</h2>
          {data.packages.length === 0 ? (
            <p className="empty">Nema paketa.</p>
          ) : (
            <table>
              <thead>
                <tr>
                  <th>Naziv</th>
                  <th>Ekosistem</th>
                  <th>Opis</th>
                </tr>
              </thead>
              <tbody>
                {data.packages.map((p, i) => (
                  <tr key={`${p.ecosystem}-${p.name}-${i}`}>
                    <td className="mono">{p.name ?? '-'}</td>
                    <td>{p.ecosystem ?? '-'}</td>
                    <td>{p.description ?? '-'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </section>
      </div>
      {modalOpen && (
        <ProjectModal
          existingNames={projects.map((p) => p.name ?? '')}
          onClose={() => setModalOpen(false)}
          onCreate={(project) => {
            setCreatedProjects((prev) => [project, ...prev])
            setModalOpen(false)
          }}
        />
      )}
    </>
  )
}

export default function App() {
  const [data, setData] = useState<DashboardData | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(true)
  const [reloadKey, setReloadKey] = useState(0)

  useEffect(() => {
    let cancelled = false
    loadDashboardData()
      .then((d) => {
        if (!cancelled) setData(d)
      })
      .catch((e: unknown) => {
        if (!cancelled) setError(e instanceof Error ? e.message : 'Nepoznata greska.')
      })
      .finally(() => {
        if (!cancelled) setLoading(false)
      })
    return () => {
      cancelled = true
    }
  }, [reloadKey])

  return (
    <div className="app">
      <header className="top">
        <div>
          <h1>Software Dependency Analyzer</h1>
          <p className="sub">Zavisnosti, ranjivosti i putanje koje do njih vode</p>
        </div>
        <div className="top-actions">
          <span className={`source ${USE_REAL_DATA ? 'source-live' : 'source-mock'}`}>
            <i /> {USE_REAL_DATA ? 'Pravi podaci (Neo4j)' : 'Mock podaci'}
          </span>
          <button
            className="refresh"
            onClick={() => {
              setLoading(true)
              setError(null)
              setReloadKey((k) => k + 1)
            }}
            disabled={loading}
          >
            Osvezi
          </button>
        </div>
      </header>

      {error && (
        <div className="error" role="alert">
          <strong>Greska pri ucitavanju.</strong> {error}
          <div className="hint">
            Za rad bez backenda postavi <code>VITE_USE_REAL_DATA=false</code> u <code>.env</code>.
          </div>
        </div>
      )}
      {loading && !data && <p className="loading">Ucitavanje...</p>}
      {data && !error && <Dashboard data={data} />}
    </div>
  )
}
