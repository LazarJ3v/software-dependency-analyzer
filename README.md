# Software Dependency Analyzer

A web application for analyzing software package dependencies and identifying potential security vulnerabilities using graph-based data modeling.

## About the Project

Software Dependency Analyzer models relationships between software projects, packages, package versions, and known vulnerabilities. It uses Neo4j to represent dependencies as a graph, making it possible to explore direct and transitive dependencies and identify projects potentially affected by vulnerable packages.

This project is developed as part of the Advanced Databases course at the University of Electronic Engineering in Nis, Serbia.

## Features

- Analyze direct and transitive software dependencies.
- Represent package relationships using a graph database.
- Identify packages affected by known security vulnerabilities.
- Trace dependency paths leading to vulnerable packages.
- Find projects that depend on affected package versions.
- Visualize dependency relationships.
- Generate dependency analysis reports.

## Technology Stack

- **Backend:** C# and ASP.NET Core Web API
- **Database:** Neo4j
- **Vulnerability Data:** OSV API

## Project Structure

```
software-dependency-analyzer/
├── sda/
├── docs/
└── README.md
```
- **sda** - software dependency analyzer API
- **docs** - project documentations 
- **README.md** - introduction to software dependency analyzer project

*The directory structure will be updated as the project develops.*