
---

# ## 🚀 Arquitectura Escalable de Endpoints con .NET Core

### *Trabajo Integrador*

---

## 1. El Desafío Común en la Industria

En aplicaciones empresariales tradicionales, el crecimiento de usuarios y datos suele degradar el rendimiento del sistema debido a:

* **Monolitos acoplados:** Controladores gigantes con lógica de negocio, validación y acceso a datos mezclados.
* **Saturación de la Base de Datos:** Consultas repetitivas bajo alta concurrencia que bloquean el rendimiento de la interfaz web (UI).
* **Validación ineficiente:** Procesamiento de lógica de negocio con datos corruptos o inválidos debido a validaciones tardías o acopladas.

---

## 2. Arquitectura Implementada: Los 3 Pilares

Para resolver estos problemas, el proyecto se diseñó bajo un enfoque de **Clean Architecture** e **Inversión de Control**, implementando tres tecnologías clave:

* **CQRS con MediatR:** Segregación de Responsabilidades de Consulta y Comando mediante un mediador en memoria.
* **Pipeline de Validación Automática:** `FluentValidation` interceptando las peticiones antes de su ejecución.
* **Caché Híbrido Avanzado:** Implementación de `HybridCache` de .NET para la protección de la base de datos.

---

## 3. Desglose Técnico y Ventajas

### 🔹 Pilar 1: CQRS con MediatR (Desacoplamiento Absoluto)

En lugar de utilizar controladores tradicionales sobrecargados, se migró a **Minimal APIs** combinadas con el patrón **CQRS** a través de la librería MediatR.

* **Cómo funciona:** Los endpoints reciben los datos y actúan únicamente como "pasamanos", delegando la ejecución a *Handlers* específicos (un Handler para crear, uno para listar, etc.).
* **Ventajas técnicas:**
* **Controladores "Flacos":** Código limpio y legible en la capa de entrada (HTTP).
* **Principio de Responsabilidad Única (SOLID):** Cada flujo de negocio vive en su propia clase aislada.
* **Mantenibilidad:** Facilita el trabajo en equipo en proyectos grandes, reduciendo los conflictos en Git.



---

### 🔹 Pilar 2: Validación en Capa de Aplicación (FluentValidation)

Se rechazó el uso de *Data Annotations* (`[Required]`, `[StringLength]`) en los modelos de base de datos para mover las reglas de validación a la capa de aplicación mediante un **Pipeline Behavior**.

* **Cómo funciona:** Un middleware interno de MediatR intercepta el comando de forma genérica. Si los datos no cumplen las reglas de `FluentValidation`, la petición se frena inmediatamente y se retorna un `400 Bad Request`, **sin llegar a tocar el Handler ni la Base de Datos**.
* **Ventajas técnicas:**
* **Validaciones Asíncronas Complejas:** Permite consultar la base de datos (ej. verificar si un correo ya existe) antes de procesar la petición.
* **Modelos Limpios:** Las entidades de dominio se mantienen puras, sin lógica visual ni de transporte pegada a ellas.
* **Flexibilidad por Contexto:** Un mismo objeto puede validarse de formas totalmente distintas según la acción (Crear vs. Modificar).



---

### 🔹 Pilar 3: Estrategia de Mitigación de Carga (`HybridCache`)

Para evitar el colapso de la base de datos bajo alta concurrencia (como el escenario de degradación por consultas masivas), se implementó el nuevo sistema de **HybridCache**.

* **Cómo funciona:** Combina la velocidad del caché en memoria (L1) con un sistema de bloqueo de concurrencia nativo (*Cache Stampede Mitigation*).
* **Ventajas técnicas:**
* **Protección del Servidor:** Si 500 usuarios solicitan el mismo reporte al mismo tiempo y no está en caché, `HybridCache` bloquea las peticiones simultáneas, realiza **una sola consulta** a la base de datos y le distribuye el resultado a los 500 usuarios.
* **Eficiencia:** Reduce drásticamente el uso de CPU y memoria del servidor de base de datos.



---

## 4. Conclusión / El Impacto en el Negocio

La implementación de esta arquitectura demuestra que el desarrollo de software moderno no consiste solo en "hacer que el código funcione", sino en diseñar sistemas preparados para el mundo real:

1. **Escalable:** Listo para soportar picos de tráfico gracias a la gestión eficiente del caché y la asincronía nativa de .NET.
2. **Seguro:** Ningún dato inválido o inconsistente logra avanzar hacia el núcleo del sistema.
3. **Enterprise-Ready:** Estructura de código idéntica a la utilizada por grandes empresas de desarrollo de software, facilitando el testing unitario y la evolución del producto a largo plazo.

---

### 💡 Consejo para tu discurso durante la presentación:

Cuando pases por la sección de **FluentValidation**, hacé énfasis en que *«las Data Annotations están bien para tutoriales o proyectos escolares simples, pero que en entornos profesionales acoplan el dominio y no permiten validaciones asíncronas contra la base de datos de forma limpia»*. Con ese tipo de comentarios es con los que vas a demostrar que manejás conceptos de la industria.