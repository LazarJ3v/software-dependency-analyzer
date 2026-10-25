import type { DashboardData } from '../types'

export const mockData: DashboardData = {
  projects: [
    { name: 'shop-web', ecosystem: 'npm', createdAt: '2026-09-02T09:15:00' },
    { name: 'billing-api', ecosystem: 'NuGet', createdAt: '2026-09-14T13:40:00' },
    { name: 'ml-pipeline', ecosystem: 'PyPI', createdAt: '2026-10-01T08:05:00' },
    { name: 'inventory-service', ecosystem: 'Maven', createdAt: '2026-10-11T16:30:00' },
  ],
  packages: [
    { name: 'react', ecosystem: 'npm', description: 'Biblioteka za izradu korisnickih interfejsa.' },
    { name: 'minimist', ecosystem: 'npm', description: 'Parsiranje argumenata komandne linije.' },
    { name: 'Newtonsoft.Json', ecosystem: 'NuGet', description: 'JSON framework za .NET.' },
    { name: 'System.Text.Encodings.Web', ecosystem: 'NuGet', description: 'Enkoderi za web sadrzaj.' },
    { name: 'pillow', ecosystem: 'PyPI', description: 'Biblioteka za obradu slika.' },
    { name: 'numpy', ecosystem: 'PyPI', description: 'Numericko racunanje u Pythonu.' },
    { name: 'log4j-core', ecosystem: 'Maven', description: 'Apache Log4j implementacija.' },
    { name: 'jackson-databind', ecosystem: 'Maven', description: 'JSON data-binding za Javu.' },
  ],
  vulnerabilities: [
    { id: 1, summary: 'Prototype pollution u minimist pri parsiranju argumenata.', severity: 'critical', fixedVersion: '1.2.6' },
    { id: 2, summary: 'Udaljeno izvrsavanje koda preko JNDI lookup-a u Log4j.', severity: 'critical', fixedVersion: '2.17.1' },
    { id: 3, summary: 'Deserijalizacija nepouzdanih podataka u jackson-databind.', severity: 'high', fixedVersion: '2.13.4.2' },
    { id: 4, summary: 'Prekoracenje bafera pri dekodiranju slika u Pillow.', severity: 'medium', fixedVersion: '10.0.1' },
    { id: 5, summary: 'Neispravno kodiranje izlaza moze dovesti do XSS-a.', severity: 'low', fixedVersion: '8.0.1' },
  ],
  paths: [
    { project: 'shop-web', vulnerabilityId: 1, path: ['shop-web', 'react-scripts@5.0.1', 'webpack-dev-server@4.11.1', 'minimist@1.2.0'] },
    { project: 'shop-web', vulnerabilityId: 1, path: ['shop-web', 'mkdirp@0.5.5', 'minimist@1.2.0'] },
    { project: 'inventory-service', vulnerabilityId: 2, path: ['inventory-service', 'spring-boot-starter-logging@2.5.0', 'log4j-core@2.14.1'] },
    { project: 'inventory-service', vulnerabilityId: 3, path: ['inventory-service', 'spring-web@5.3.9', 'jackson-databind@2.12.3'] },
    { project: 'ml-pipeline', vulnerabilityId: 4, path: ['ml-pipeline', 'torchvision@0.15.0', 'pillow@9.4.0'] },
    { project: 'billing-api', vulnerabilityId: 5, path: ['billing-api', 'Microsoft.AspNetCore.App@3.1', 'System.Text.Encodings.Web@4.7.0'] },
  ],
}
