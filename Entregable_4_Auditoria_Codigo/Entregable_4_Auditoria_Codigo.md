# Entregable 4: Auditoría Técnica C#

**Contexto:** Revisión del código del Endpoint `AssignProvider` solicitado en la prueba técnica.
**Autoridad de Revisión:** Líder Técnico del Squad.

---

## 1. El Código bajo Auditoría (Código Original)

```csharp
[HttpPost("assign")]
public async Task<IActionResult> AssignProvider(Request request) 
{
    var providers = _db.Providers.ToList();
    var selected = providers.FirstOrDefault(); // escoger el primero nomas
    if(selected == null) return BadRequest("No providers");

    selected.IsBusy = true;
    _db.SaveChanges();

    return Ok(selected);
}
```

---

## 2. Diagnóstico

En la auditoría del código, detectamos **5 patrones inaceptables** que en Producción ocasionarían caídas sistémicas, latencias extremas y fallos operativos. El feedback es el siguiente:

1.  **Desperdicio de RAM (`Select N+1`):** La instrucción `_db.Providers.ToList()` arrastra a la memoria del servidor Web la **totalidad de la base de datos** para inmediatamente después escoger solo uno (`FirstOrDefault()`). Toda búsqueda debe empujarse obligatoriamente al motor relacional SQL.
2.  **Falsa Asincronía y Deadlocks:** El método firma como `async Task`, pero usa lógicas estrictamente síncronas bloqueantes (`.ToList()`, `_db.SaveChanges()`). En horas pico de trafico, estos hilos retenidos agotarán el *Thread Pool* de IIS/Kestrel.
3.  **Filtrado Ciego y Ruptura de Negocio:** El comentario empírico `// escoger el primero nomas` expone una carencia analítica severa. El código nunca verifica si la grúa ya está ocupada (`IsBusy`). Se terminaría asignando el mismo proveedor a docenas de vehículos siniestrados en bucle.
4.  **Acoplamiento Férreo (Falta de IoC y Servicios):** El Controlador Web accede directamente a la instancia cruda de base de datos (`_db`). Esto viola el pilar de Clean Architecture del encapsulamiento e imposibilita realizar Test Unitarios verdaderos. Es obligatorio aislar esta interacción a través de una Interfaz (Repository) o hacia la Capa de Servicios de Dominio.
5.  **Fuga de Datos Sensibles (Ruptura DTO):** El endpoint cierra con `return Ok(selected)`, eyectando libremente por Internet una Entidad Cruda de Base de Datos. Esto expone toda la estructura de Entity Framework, y vulnera los campos y llaves a inyecciones.


---

## 3. Feedback Táctico Transaccional

Alterar la disponibilidad de un proveedor (`IsBusy = true`) es una **operación estructuralmente crítica** para el flujo en calle. Este movimiento de piezas jamás debe ser ignorado como una simple propiedad en código; debe estar inherentemente atado a un **Log de Auditoría de Sistemas** empleando Registros Estructurados. Se debe dejar constancia exacta de qué Ticket (Contexto) efectuó el bloqueo y en qué momento preciso, para poder inyectarlo en *AWS CloudWatch* previniendo denuncias o bloqueos fantasmas en la flota.

---

## 4. Reescritura Magistral (Refactorización a Nivel Senior)

El siguiente código expone cómo debe aislarse y delegarse la lógica bajo los estándares propuestos. 

```csharp
[HttpPost("assign")]
public async Task<IActionResult> AssignProvider([FromBody] AssignProviderRequestDto request, CancellationToken cancellationToken) 
{
    // Validación de Contrato Defensiva Inicial
    if (request == null || !ModelState.IsValid)
        return BadRequest(new { Error = "Payload inválido o nulo. Contrato de entrada no verificado." });

    try
    {
        // 1. Filtrado Eficiente: Delegado a la Capa de Servicios
        // El cómputo del Query Async ocurre en la Base de Datos.
        var selectedProvider = await _assignmentAppService.GetAvailableProviderAsync(request, cancellationToken);

        if (selectedProvider == null) 
            return NotFound(new { MatchSuccess = false, Error = "No contamos con proveedores disponibles operativos para su tipología." });

        // 2. Operación Crítica Transaccional 
        selectedProvider.IsBusy = true;
        
        // 3. Persistencia pura Non-Blocking Async
        await _assignmentAppService.CommitAssignmentAsync(selectedProvider, cancellationToken);

        // 4. Mapeo vía Patrón DTO protegiendo la Entidad EF
        var responseDto = new ProviderAssignedDto 
        {
            ProviderId = selectedProvider.Id,
            ProviderName = selectedProvider.Name,
            AssignmentTimeUtc = DateTime.UtcNow
        };

        return Ok(responseDto);
    }
    catch (DbUpdateConcurrencyException)
    {
        // Resiliencia a carrera de recursos simultánea
        return Conflict(new { MatchSuccess = false, Error = "Colisión Operativa. El proveedor acaba de ser adjudicado por nuestra mesa de enlace simultáneamente." });
    }
}
```

> [!CAUTION] 
> **Nota Avanzada sobre Arquitectura y Concurrencia Optimista (EF Core):**
> Para asegurar que el bloque de mitigación `catch (DbUpdateConcurrencyException)` levante y bloquee el embudo previniendo que **dos clientes asíncronos logren adjudicarse la misma grúa**, es un requisito obligatorio que la entidad `Provider` posea embebido un marcador de versión de fila (`RowVersion` mapeado como `[Timestamp]` o FluentApi `.IsRowVersion()`). 
> 
> Sin el **Concurrency Token Optimista** inicializado en Base de Datos, Entity Framework ejecutará un escenario en silencioso. Las lógicas pisarían la actualización anulando los seguros de protección operativa e induciendo a los proveedores a una confusión en campo.