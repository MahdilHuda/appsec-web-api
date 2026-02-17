## Assets & Threat Model

### Product Data Integrity  
*Description:*  
The enforcement of server authority over product state and financial constraints.  
This ensures that server-controlled identifiers and financial fields cannot be manipulated via client-provided input or unauthorized state mutation.

**Vulnerability Mapping**  
Specifically addresses mass assignment (overwriting internal fields) and 
Business Logic Flaws (accepting negative price values).

**Business Impact:**
- **Direct Financial loss:** Manipulation of price fields allows transactions below cost or negative totals.
- **Data Corruption:** Unauthorized modification of server-controlled identifiers leads to inconsistent states across internal databases and ERP systems.
- **Downstream Failure:** Broken synchronization with logistics and inventory systems due to corrupted product IDs.  

  
### Service Availability  
*Description:*  
The resilience of the API against resource exhaustion and its ability to handle anomalous input without service interruption.

**Vulnerability Mapping**  
Specifically addresses unhandled exceptions (causing service crashes) and 
application-level DoS via repeated processing of malformed or non-existent IDs.

**Business Impact:**
- **Systemic Instability:** Unhandled exceptions can lead to "Zombie" processes or full service restarts, causing intermittent downtime.
- **Denial of Service (DoS):** Resource-heavy operations triggered by invalid inputs can exhaust CPU/Memory, making the API unavailable to legitimate users.
- **SLA Violations:** Failure to meet contractual uptime and responsiveness targets, resulting in financial penalties.


### API Trust Boundary Integrity  
*Description:*  
The rigorous enforcement of the API contract at the ingress point.  
This ensures the server remains the sole arbiter of business rules by validating all incoming data against strict schemas before processing.

**Vulnerability Mapping**  
Addresses Client-controlled IDs and Missing Input Validation.

**Business Impact:**
- **Unauthorized Object Manipulation:** Inadequate boundary checks allow clients to dictate object state or identifiers that should be server-generated.
- **Business Rule Bypass:** Input that bypasses validation allows the injection of data that violates internal logic.
- **Inconsistent System State:** Acceptance of malformed data at the boundary leads to "garbage-in, garbage-out" scenarios in the application layer.


### Operational Visibility & Error Hygiene
*Description:*  
The system’s ability to provide structured, secure logging and controlled error responses.  
This ensures that the system remains observable for monitoring and incident response without leaking sensitive architectural details.

**Vulnerability Mapping**  
Specifically addresses unhandled exceptions and the resulting stack traces or generic error leakage.

**Business Impact:**
- **Increased MTTD/MTTR:** Poorly handled exceptions (crashes without logs) hide the root cause, delaying detection and recovery during an incident.
- **Information Disclosure:** Verbose error messages or stack traces reveal internal file paths, framework versions, or database schemas to potential attackers.
- **Monitoring Blind Spots:** Failure to log failed validation attempts prevents the SOC (Security Operations Center) from identifying active probing or automated attacks.


## Trust Boundaries
```
┌──────────────────────────────────┐
│         Untrusted Client         │
│    (Postman / Browser / Script)  │
└────────────────┬─────────────────┘
                 │
                 │ HTTP + JSON
                 ▼
┌──────────────────────────────────┐
│       ASP.NET Core Web API       │
│   (Controllers + Model Binding)  │
└────────────────┬─────────────────┘
                 │
                 │ In-process call
                 ▼
┌──────────────────────────────────┐
│     In-Memory Product Store      │
│  (List<Product> / Static Coll.)  │
└──────────────────────────────────┘
```
> * **TB1: Client → API:** This represents the HTTP boundary where untrusted user input enters the trusted execution environment. It's the primary point for validation and sanitization.
> * **TB2: API → Data Store:** An in-process boundary representing a trusted internal call. The system must remain cautious as data could already be "poisoned" if TB1 checks were insufficient.
> * **TB3 (Future): API → Database:** This will represent the External Service boundary, where data leaves the local process to interact with a persistent database or third-party API, introducing new requirements for network security and serialization.  
  
### Boundary Failure Mapping

| Vulnerability | Where boundary fails | What is trusted incorrectly | Asset impacted |
| :--- | :--- | :--- | :--- |
| **Mass assignment** | **TB1** (Model Binding) | DTO shape allows client-provided fields that the server should exclusively control. | Product Data Integrity / API Trust |
| **Missing validation** | **TB1** (Controller/Handler) | Controller/handler accepts Name or Price without guard clauses or validation attributes. | Product Data Integrity / API Trust |
| **Unhandled exception** | **TB1** (Request Pipeline) | Route param lookup fails due to missing "Not Found" handling or unmanaged indexing errors. | Availability / Error Hygiene |

### Attack Surface
**POST /products** - Unauthenticated JSON ingestion  
  
*Input Channels:*  
Request Body (JSON) and Headers.  
  
*Attack Vector:*  
- *Over-posting:* Client sends a manually defined Id to break server authority over identity.
- *Data poisoning:* Client sends `price = -1` or `name = ""` to inject invalid business states.
  
**GET /products/{id}** — Parameter-driven lookup  
  
*Input Channels:*  
Route parameter {id}.
  
*Attack Vector:*  
- *Non-existent/Out-of-range ID:* Attacker sends IDs that don't exist or are extreme values to trigger crashes (500 errors).
- *Type Mismatch:* Sending non-numeric strings to test binder robustness and error leakage.

### Exploit Chain Analysis
An attacker uses over-posting to control the Id and injects a negative price via POST /products, poisoning the in-memory store.  
When a legitimate consumer or downstream system fetches this product, it causes incorrect total calculations and financial loss.  
Simultaneously, the attacker spams GET requests with non-existing IDs, triggering 500 Internal Server Errors that mask the poisoning attack with monitoring noise and cause service instability.


## STRIDE Threat Classification

| Vulnerability | STRIDE | Justification | Realistic Impact | Threat Actor Profile |
| :--- | :--- | :--- | :--- | :--- |
| **Mass Assignment** | **Tampering** | Trust boundary enforcement fails. Client controls server-authoritative fields during model binding. | Unauthorized state mutation. Attacker overwrites existing records or bypasses identity logic. | **Low-skill Attacker:** Uses manual JSON manipulation to probe for hidden fields. |
| **Missing Validation** | **Tampering** | Domain invariants are not enforced at boundary crossing. Invalid state transitions are accepted into the trusted store. | Data poisoning. Negative prices or empty strings break downstream accounting/calculation systems. | **Opportunistic attacker/Automated Bot:** Systematically testing boundary conditions. |
| **Unhandled Exception** | **Information Disclosure / Denial of Service** | Unhandled exceptions at boundary crossing propagate through the request pipeline without controlled error handling. Info disclosure is conditional if detailed errors/stack traces are enabled. | Service instability (DoS) or leakage of internal code paths and framework versions to the client. | **Low-skill Attacker:** Performing boundary-case probing. |  

### Key learnings
- HTTP is a hard trust boundary: client input must be treated as untrusted, and the server must remain authoritative over identifiers and state.
- Missing validation is not just “bad data” — it’s tampering against domain invariants that can cause real business impact (e.g., financial loss, corrupted downstream calculations).
- Unhandled exceptions are security-relevant: they can destabilize availability and, if error details are misconfigured, disclose internal implementation signals that aid attackers.


