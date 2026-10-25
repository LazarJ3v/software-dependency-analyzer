import { useEffect, useRef, useState } from 'react'
import type { FormEvent } from 'react'
import type { Project } from './types'

const MAX_LOCK_SIZE = 20 * 1024 * 1024 // 20 MB

interface Props {
  existingNames: string[]
  onClose: () => void
  onCreate: (project: Project) => void
}

// Proverava da je sadrzaj validan package-lock.json i vraca broj zavisnosti.
function countLockDependencies(text: string): number {
  let json: unknown
  try {
    json = JSON.parse(text)
  } catch {
    throw new Error('Fajl nije ispravan JSON.')
  }
  if (typeof json !== 'object' || json === null) {
    throw new Error('Fajl ne izgleda kao package-lock.json.')
  }
  const lock = json as {
    packages?: Record<string, unknown>
    dependencies?: Record<string, unknown>
  }
  if (lock.packages && typeof lock.packages === 'object') {
    // Kljuc "" je sam projekat, ne zavisnost.
    return Object.keys(lock.packages).filter((k) => k !== '').length
  }
  if (lock.dependencies && typeof lock.dependencies === 'object') {
    return Object.keys(lock.dependencies).length
  }
  throw new Error('Fajl ne sadrzi "packages" ni "dependencies" - nije package-lock.json.')
}

export default function ProjectModal({ existingNames, onClose, onCreate }: Props) {
  const dialogRef = useRef<HTMLDialogElement>(null)
  const [name, setName] = useState('')
  const [file, setFile] = useState<File | null>(null)
  const [errors, setErrors] = useState<{ name?: string; file?: string }>({})
  const [submitting, setSubmitting] = useState(false)

  useEffect(() => {
    const dialog = dialogRef.current
    if (dialog && !dialog.open) dialog.showModal()
  }, [])

  async function handleSubmit(e: FormEvent) {
    e.preventDefault()
    const next: { name?: string; file?: string } = {}
    const trimmed = name.trim()

    if (!trimmed) next.name = 'Naziv projekta je obavezan.'
    else if (existingNames.some((n) => n.toLowerCase() === trimmed.toLowerCase()))
      next.name = 'Projekat sa ovim nazivom vec postoji.'

    let dependencyCount = 0
    if (!file) {
      next.file = 'Fajl package-lock.json je obavezan.'
    } else if (file.name.toLowerCase() !== 'package-lock.json') {
      next.file = 'Fajl mora da se zove package-lock.json.'
    } else if (file.size > MAX_LOCK_SIZE) {
      next.file = 'Fajl je prevelik (maksimum 20 MB).'
    } else {
      try {
        dependencyCount = countLockDependencies(await file.text())
      } catch (err) {
        next.file = err instanceof Error ? err.message : 'Fajl se ne moze procitati.'
      }
    }

    setErrors(next)
    if (next.name || next.file || !file) return

    setSubmitting(true)
    // Samo frontend: projekat se dodaje u lokalno stanje, nista se ne salje backendu.
    onCreate({
      name: trimmed,
      ecosystem: 'npm',
      createdAt: new Date().toISOString(),
      lockFileName: file.name,
      dependencyCount,
    })
  }

  return (
    <dialog
      ref={dialogRef}
      className="modal"
      onClose={onClose}
      onClick={(e) => {
        // Klik na pozadinu (van sadrzaja) zatvara prozor.
        if (e.target === dialogRef.current) dialogRef.current?.close()
      }}
      aria-labelledby="project-modal-title"
    >
      <form className="modal-body" onSubmit={handleSubmit} noValidate>
        <h2 id="project-modal-title">Novi projekat</h2>

        <label className="field">
          <span>Naziv projekta</span>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            placeholder="npr. shop-web"
            autoFocus
            aria-invalid={Boolean(errors.name)}
          />
          {errors.name && <em className="field-error">{errors.name}</em>}
        </label>

        <label className="field">
          <span>Ekosistem</span>
          <input type="text" value="npm" disabled />
          <small>Za package-lock.json ekosistem je uvek npm.</small>
        </label>

        <label className="field">
          <span>Fajl package-lock.json</span>
          <input
            type="file"
            accept=".json,application/json"
            onChange={(e) => setFile(e.target.files?.[0] ?? null)}
            aria-invalid={Boolean(errors.file)}
          />
          {file && <small className="mono">{file.name}</small>}
          {errors.file && <em className="field-error">{errors.file}</em>}
        </label>

        <div className="modal-actions">
          <button type="button" className="secondary" onClick={() => dialogRef.current?.close()}>
            Otkazi
          </button>
          <button type="submit" className="refresh" disabled={submitting}>
            Kreiraj projekat
          </button>
        </div>
      </form>
    </dialog>
  )
}
