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



