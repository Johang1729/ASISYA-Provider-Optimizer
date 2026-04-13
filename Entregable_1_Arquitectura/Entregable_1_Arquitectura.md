# ENTREGABLE 1: Diseño Arquitectónico - ASISYA

Para este proyecto, he definido una arquitectura de Microservicios apoyada en Clean Architecture y Domain-Driven Design (DDD). Busco garantizar que la plataforma no solo sea escalable, sino que soporte fallos críticos sin comprometer la operación en AWS.

## 1. Visión General del Sistema (C4 Nivel 1)

En este primer nivel, podemos ver el contexto global de ASISYA. Aquí se identifican los actores principales (el cliente que pide ayuda y el proveedor que asiste) y las dependencias directas con los servicios de terceros, como la pasarela de pagos, el proveedor de identidad y los motores de mapas.

```mermaid
flowchart TD
    %% Estilos base
    classDef person fill:#08427b,color:#fff,stroke:#052e56,stroke-width:2px;
    classDef system fill:#1168bd,color:#fff,stroke:#0b4884,stroke-width:2px;
    classDef extSystem fill:#999999,color:#fff,stroke:#6b6b6b,stroke-width:2px;

    %% Actores
    C["👤 Cliente<br/><br/>Solicita asistencia<br/>vehicular desde la app."]:::person
    P["👤 Proveedor<br/><br/>Grúa/Taller que<br/>atiende el servicio."]:::person

    %% Core
    S["⚙️ Plataforma ASISYA<br/><br/>Gestión de asignaciones,<br/>tracking y alertas."]:::system

    %% Dependencias externas
    I["Auth Provider<br/><br/>Autenticación OAuth2/JWT."]:::extSystem
    B["Billing / CRM<br/><br/>Valida planes (Básico/Premium)."]:::extSystem
    M["Google Maps API<br/><br/>Cálculo de tiempos y tráfico."]:::extSystem

    %% Interacciones
    C -->|Solicita servicio| S
    P <-->|Alertas y estados| S

    S -->|Verifica Token| I
    S -->|Consulta plan| B
    S -->|Calcula ETA| M
```

## 2. Arquitectura de Contenedores y Nube (C4 Nivel 2)

Para la infraestructura en AWS, decidí separar las responsabilidades en microservicios hiper focalizados. Todo se comunica de forma asíncrona mediante un bus de eventos (Message Broker) para mantener los componentes desacoplados.

```mermaid
flowchart LR
    %% Estilos de la arquitectura
    classDef person fill:#08427b,color:#fff,stroke:#052e56,stroke-width:2px,rx:5px,ry:5px;
    classDef system fill:#1168bd,color:#fff,stroke:#0b4884,stroke-width:2px;
    classDef extSystem fill:#555555,color:#fff,stroke:#333,stroke-width:2px;
    classDef db fill:#e67e22,color:#fff,stroke:#d35400,stroke-width:2px,rx:10px,ry:10px;
    classDef queue fill:#27ae60,color:#fff,stroke:#1e8449,stroke-width:2px;
    classDef obs fill:#8e44ad,color:#fff,stroke:#5b2c6f,stroke-width:2px;

    %% Frontend / Móvil
    C["📱 App Cliente"]:::person
    P["📱 App Proveedor"]:::person

    %% Gateways
    ALB["⚖️ API Gateway / ALB<br/>(WAF y Ruteo)"]:::system
    CDN["🌐 CloudFront (CDN)<br/>(Caché estático)"]:::system

    %% APIs de Terceros
    EXT["🔑 Third-Party APIs<br/>(Auth / CRM / Maps)"]:::extSystem
    
    %% Microservicios (.NET)
    REQ["⚙️ Request Service<br/>(Gestión de Tickets)"]:::system
    OPT["🧠 Optimizer Service<br/>(Motor de Asignación)"]:::system
    LOC["📡 Location Service<br/>(Tracking 10Hz)"]:::system
    NOT["🔔 Notification Service<br/>(Alertas y Push)"]:::system

    %% Infra y Datos
    SQS["📨 Message Broker<br/>(AWS SQS / SNS)"]:::queue
    OBS["👁️ Observabilidad<br/>(CloudWatch / X-Ray)"]:::obs
    PG[("🐘 RDS Postgres<br/>(Data Transaccional)")]:::db
    RD[("⚡ Redis Cache<br/>(GeoHashes en vivo)")]:::db
    S3[("🪣 AWS S3<br/>(Evidencia fotográfica)")]:::db

    %% Flujos de Red
    C -->|API Calls| ALB
    P -->|Envía GPS/Estados| ALB
    C -->|Sube fotos| CDN
    CDN -->|Almacena| S3

    ALB -.->|Verifica| EXT
    ALB --->|Rutea| REQ
    ALB --->|Rutea| OPT
    ALB --->|Rutea| LOC

    %% Persistencia y Trazabilidad
    REQ --->|Guarda ticket| PG
    OPT --->|Lee historial| PG
    OPT --->|Busca cercanos| RD
    LOC --->|Actualiza GPS| RD
    REQ -.->|Logs/Traces| OBS
    OPT -.->|Logs/Traces| OBS

    %% Bus de Eventos
    REQ -->|Publica evento| SQS
    OPT -->|Publica evento| SQS
    SQS -->|Consume| NOT

    %% Retorno
    NOT -.->|Push notification| C
    NOT -.->|Push notification| P
```

## 3. Estructura Interna del Motor de Optimización (C4 Nivel 3)

Me enfoqué en aplicar Clean Architecture de forma estricta para el servicio core de asignación (`ProviderOptimizerService`). Esto me permite asegurar que toda la lógica de negocio (nuestro scoring de variables) quede totalmente aislada y no dependa de la base de datos o de librerías externas.

```mermaid
flowchart TD
    %% Estilos de Clean Architecture
    classDef web fill:#1168bd,color:#fff,stroke:#0b4884,stroke-width:2px;
    classDef app fill:#27ae60,color:#fff,stroke:#1e8449,stroke-width:2px;
    classDef domain fill:#d35400,color:#fff,stroke:#a04000,stroke-width:2px;
    classDef infra fill:#7f8c8d,color:#fff,stroke:#616a6b,stroke-width:2px;
    classDef interface fill:#8e44ad,color:#fff,stroke:#5b2c6f,stroke-width:2px,stroke-dasharray: 5 5;

    %% Fondos transparentes
    style OPT_SERVICE fill:none,stroke:#fff,stroke-width:2px,stroke-dasharray: 5 5
    style WebLayer fill:none,stroke:#1168bd,stroke-width:2px
    style AppLayer fill:none,stroke:#27ae60,stroke-width:2px
    style Interfaces fill:none,stroke:#8e44ad,stroke-width:2px,stroke-dasharray: 5 5
    style DomainLayer fill:none,stroke:#d35400,stroke-width:2px
    style InfraLayer fill:none,stroke:#7f8c8d,stroke-width:2px

    subgraph OPT_SERVICE ["🧠 ProviderOptimizerService (Componentes)"]
        direction TD

        subgraph WebLayer ["Capa de Presentación (API)"]
            CTRL1["OptimizerController<br/>(POST /optimize)"]:::web
            CTRL2["ProvidersController<br/>(GET /providers/available)"]:::web
        end

        subgraph AppLayer ["Capa de Aplicación (Use Cases)"]
            UC1["FindOptimalProviderUseCase<br/>(Calcula y asigna)"]:::app
            UC2["BroadcastBoardUseCase<br/>(Notifica al radar)"]:::app
            
            subgraph Interfaces ["Puertos / Contratos"]
                IREPO["IProviderRepository"]:::interface
                IGEO["IGeoCacheService"]:::interface
                IEVENT["IEventPublisher"]:::interface
            end
        end

        subgraph DomainLayer ["Capa de Dominio (Core)"]
            ENT["Provider Entity<br/>(Root Aggregate)"]:::domain
            RULES["ScoringPolicy<br/>(Lógica de Matriz Ponderada)"]:::domain
            VO["Location ValueObject<br/>(Lat/Lng)"]:::domain
        end

        subgraph InfraLayer ["Capa de Infraestructura"]
            REPO["PostgresProviderRepository<br/>(EF Core)"]:::infra
            GEO["RedisGeoClient<br/>(StackExchange.Redis)"]:::infra
            PUB["SnsSqsEventPublisher<br/>(MassTransit)"]:::infra
        end
    end

    %% Relaciones
    CTRL1 -->|Llama| UC1
    CTRL2 -->|Llama| UC2

    UC1 -->|Aplica en| ENT
    UC1 -->|Usa reglas| RULES
    UC2 -->|Usa| VO

    %% Inyección de dependencias
    UC1 -.->|Inyecta| IREPO
    UC1 -.->|Inyecta| IGEO
    UC1 -.->|Inyecta| IEVENT

    REPO -.->|Implementa| IREPO
    GEO -.->|Implementa| IGEO
    PUB -.->|Implementa| IEVENT
```

## 4. Modelado de Microservicios (DDD)

Para garantizar la escalabilidad técnica y de equipo, dividí la plataforma en cuatro Bounded Contexts. A continuación detallo las responsabilidades, entidades del dominio, contratos y eventos de cada uno:

### 4.1 AssistanceRequestService
* **Responsabilidad:** Gestiona el ticket de asistencia. Se encarga del triaje inicial mediante árboles de decisión, verifica el plan del usuario (Básico, Plus, Premium) y asegura la consistencia en la Máquina de Estados del servicio.
* **Entidades (DDD):** `AssistanceTicket` (Aggregate Root), `TimelineEvent` (Log Inmutable).
* **Contratos:** `IAssistanceRepository`, `IBillingClient`.
* **Eventos:** Publica `AssistanceCreatedEvent` y `AssistanceStateChangedEvent`.

### 4.2 ProviderOptimizerService
* **Responsabilidad:**  Inicia la búsqueda con geofencing expansivo y calcula el score ponderando: Distancia, ETA en vivo, Disponibilidad, Rating histórico y Tasa de Aceptación. 
* **Lógica de Asignación:** Lanza un "Ping Directo" al mejor proveedor por 45 segundos. Si declina, activa el "Tablero de Zona (Board)" para los siguientes 5 proveedores bajo el modelo *First-come, first-served*.
* **Entidades (DDD):** `OptimizationTicket` (Aggregate Root), `ProviderScoreProfile`, `ScoringPolicy` (Domain Service).
* **Contratos:** `IProviderRepository`, `IGeoCacheService`, `IEventPublisher`.
* **Eventos:** Publica `ProviderAssignedEvent` y `BoardBroadcastTriggeredEvent`.

### 4.3 LocationService
* **Responsabilidad:** Servicio ligero enfocado en tracking. Ingesta posiciones GPS de los proveedores a alta frecuencia (10Hz) y permite búsquedas geoespaciales ultrarrápidas.
* **Entidades (DDD):** `ProviderTracer` (Aggregate Root), `GeoPoint` (Value Object).
* **Contratos:** `IRedisLocationRepository`.
* **Eventos:** No publica, solo actualiza la caché efímera.

### 4.4 NotificationsService
* **Responsabilidad:** Encargado del despacho hacia afuera (Push, SMS, WebSockets) para alertas a dispositivos móviles, sin bloquear el flujo transaccional.
* **Entidades (DDD):** `PushNotification` (Aggregate Root), `DeliveryReceipt`.
* **Contratos:** `IPushNotificationProvider`.
* **Eventos:** Es netamente consumidor (Subscriber) de los eventos de asignación y estado.

## 5. Estrategia de Escalabilidad e Infraestructura

He diseñado la infraestructura para soportar picos de demanda impredecibles (ej. condiciones climáticas extremas o de alta demanda):

* **Elasticidad (Autoscaling):** Los servicios core corren sobre pods en Kubernetes/ECS. Utilizan métricas de CPU/Memoria para escalar horizontalmente de forma automática.
* **Mensajería Asíncrona (Pub/Sub):** Utilizamos AWS SQS y SNS. Esto permite un procesamiento asincrónico real, evitando que los servicios se bloqueen esperando respuestas HTTP mutuas.
* **Fallback y Retries:** Si una dependencia externa crítica falla (ej. Google Maps API), el API Gateway y los microservicios implementan el patrón *Circuit Breaker* (Polly en .NET) realizando *retries* exponenciales. Si el fallo persiste, se ejecuta un *Fallback* interno usando algoritmos geográficos de línea recta (Haversine) para no detener la operación.
* **Patrón Outbox:** Para evitar la pérdida de eventos transaccionales ante caídas de red, la asignación en DB y la publicación en SQS se graban dentro de la misma transacción ACID de Postgres.

## 6. Estrategia de Seguridad Perimetral y OWASP

La seguridad se maneja en capas para proteger los datos sensibles y el GPS en vivo:

* **Identidad y Acceso (JWT):** El API Gateway recibe el token OAuth2, pero los microservicios lo validan internamente verificando las firmas JWKS (JSON Web Key Set).
* **WAF y Rate Limits:** AWS WAF protege contra inyecciones SQL y ataques XSS (estándares OWASP). Además, el API Gateway implementa *Rate Limiting* (Throttling) estricto para evitar abusos o DDoS en los endpoints públicos.
* **Gestión de Secretos:** Está estrictamente prohibido usar credenciales en el código. AWS Secrets Manager inyecta dinámicamente las cadenas de conexión en tiempo de despliegue.
* **Mínimo Privilegio (IAM Roles):** Cada contenedor tiene un Rol de IAM restringido. El servicio de notificaciones, por ejemplo, no tiene permisos de lectura sobre la base de datos de pagos.
* **Auditoría y Trazabilidad:** Todo cambio de estado genera un log inmutable. La observabilidad se centraliza en AWS CloudWatch y X-Ray para facilitar la auditoría técnica y el rastreo de peticiones distribuidas.