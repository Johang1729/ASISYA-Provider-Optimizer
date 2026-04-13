# ASISYA - Provider Optimizer Engine

## Descripción
**ASISYA - Provider Optimizer Engine** es un microservicio crítico de enrutamiento inteligente diseñado para el ecosistema de asistencia vehicular. Construido bajo prácticas de ingeniería optimas, esta plataforma es escalable, resiliente y altamente predecible. Su núcleo matemático evalúa múltiples variables en tiempo real (Distancia, ETA, Rating y Tasa de Aceptación Histórica) para adjudicar servicios de emergencia mediante un sistema híbrido de *Direct Ping* asíncrono y un tablero estructurado de *Fallback*.

---

## 📑 Índice de Entregables

Este repositorio consolida la solución integral para el motor de optimización de proveedores. Se ha organizado de manera modular para facilitar el proceso de auditoría técnica:

### 📖 Recursos Adicionales
Para una comprensión profunda de la visión estratégica y el diseño, se incluyen los siguientes documentos en la raíz:
- [Prueba Técnica (Original)](./PRUEBA%20TÉCNICA%20-%20Lider%20Tecnico%20(1).pdf): Requerimientos y criterios de evaluación.
- **Diagramas y Evidencias**: Cada carpeta de entregable incluye sus propios diagramas (C4, UML) y capturas de pantalla de validación.

| Entregable | Directorio | Descripción Técnica |
| :--- | :--- | :--- |
| **Entregable 1** | `Entregable_1_Arquitectura/` | Diseño de Arquitectura Cloud estratégico, Modelos C4, Outbox Pattern y topologías de resiliencia en AWS. |
| **Entregable 2** | `Entregable_2_Microservicio/` | Microservicio Core. Código fuente en **.NET 8** guiado por **Clean Architecture**, CQRS (**MediatR**) y persistencia relacional en **PostgreSQL**. |
| **Entregable 3** | `Entregable_3_Estandares_Squad/` | Manual Oficial del Squad. Directrices de **GitFlow**, **CI/CD**, control riguroso de deuda técnica (SQALE) y Observabilidad. |
| **Entregable 4** | `Entregable_4_Auditoria_Codigo/` | Auditoría Técnica profunda y Code Review, detallando refactorización defensiva contra fallas de memoria, cuellos de botella asíncronos y **concurrencia optimista**. |
| **Entregable 5** | `.github/workflows/ci-cd.yml` | Pipeline de CI/CD para GitHub Actions evaluando estándares, disparando el cluster de Testcontainers y empaquetando para **AWS ECR**. |
| **Entregable 6 (Extra)** | `Entregable_6_Frontend/` | Single Page Application (SPA) en **React + Vite** con utilidades de Tailwind. Un prototipo visual corporativo que consume el *Score Matrix* de ASISYA. |
| **Presentación** | `Presentación_Ejecutiva/` | Presentación ejecutiva consolidada del proyecto y la solución propuesta. |

---

## 🛠️ Stack Tecnológico

El microservicio fue edificado asegurando alto rendimiento y seguridad empresarial:
*   **Lenguaje & Framework:** C# 12, .NET 8 (LTS)
*   **Arquitectura Base:** Clean Architecture, Domain-Driven Design (DDD), CQRS (MediatR)
*   **Persistencia:** Entity Framework Core 8, PostgreSQL
*   **Validación & Testing (QA):** xUnit, FluentAssertions, Testcontainers
*   **DevOps & Infraestructura:** Docker (Alpine, Multi-Stage Builds), Docker Compose, GitHub Actions

---

## 🚀 Guía de Revisión Sugerida

Para una evaluación coherente de la madurez de la solución, se recomienda seguir este orden de inspección:

1.  **Visión (Entregable 1):** Revisar el diseño de arquitectura y la topología en AWS.
2.  **Calidad Operativa (Entregable 3):** Consultar el manual de estándares para entender las reglas de juego del equipo.
3.  **Core Logics (Entregable 2):** Auditar la implementación limpia del microservicio en .NET.
4.  **Auditoría de Deuda (Entregable 4):** Revisar el proceso de refactorización y control de transacciones críticas.
5.  **Experiencia de Usuario (Entregable 6):** Probar el prototipo funcional en React.

La solución está preparada para encender bajo un paradigma *Zero Configuration*.

### 1. Levantar el Backend (Infraestructura + Microservicio)
Abre una terminal y navega a la carpeta del microservicio:
```bash
cd Entregable_2_Microservicio
docker-compose up -d
```
Esto desplegará PostgreSQL y la API en contenedores de forma transparente.

### 2. Ejecutar la Suite de Pruebas Automáticas
Para constatar la política de code coverage y validar la eficacia de los Testcontainers:
```bash
dotnet test Entregable_2_Microservicio/Asisya.ProviderOptimizer.Tests/Asisya.ProviderOptimizer.Tests.csproj --verbosity normal
```

### 3. Levantar la Interfaz Visual (Frontend SPA React)
Para experimentar la simulación del radar y el algoritmo *Direct Ping* en vivo:
1. Navega a la carpeta del Frontend:
```bash
cd Entregable_6_Frontend
```
2. Instala las dependencias y arranca el entorno de desarrollo:
```bash
npm install
npm run dev
```
3. Abre en tu navegador el enlace que aparece en consola (ej. `http://localhost:5173`).

---

## 🛡️ Nota de Calidad

Este entregable ha sido estructurado considerando escenarios críticos de alta concurrencia operacional. Se mitigó agresivamente la generación de deuda técnica mediante una alta cohesión arquitectónica (Cero acceso crudo al modelo de EF Core en controladores), blindaje explícito vía *Concurrency Tokens*, un robusto esquema defensivo sin secretos filtrados (`.env`) y flujos rigurosamente asíncronos.
