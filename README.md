# AppSec Web API – Iteration 1

This repository is part of a structured Application Security / DevSecOps progression.

Iteration 1 focuses on identifying insecure API patterns, performing threat modeling, and implementing secure remediations with regression validation.

---

## Scope

ASP.NET Core Web API using in-memory storage.

Endpoints:
- `POST /products`
- `GET /products`
- `GET /products/{id}`

Authentication is intentionally omitted in this iteration to focus on trust boundaries and input handling.

---

## Vulnerabilities Introduced

The following vulnerabilities were intentionally implemented and documented:

1. Mass Assignment (client-controlled identifier)
2. Missing Input Validation (negative price / empty name)
3. Unhandled Exception (500 on missing resource)

See:  
`docs/VULNS.md`

---

## Remediation Summary

The following security controls were implemented:

- Enforced server authority over identifiers
- Implemented boundary validation of input
- Enforced domain invariants
- Replaced exception-driven control flow with proper `404 NotFound`
- Added global exception handling with standardized error responses

Threat model:  
`docs/threat-model1.md`

---

## Running Security Regression Tests

1. Import Postman collection:
`Vulns.1.postman_collection.json`
2. (Optional) Import environment:
`local.postman_environment.json`

3. Select the environment and run the **Regression** folder.

The regression tests verify that previously exploitable behaviors are no longer possible.

---

## Learning Focus

This iteration demonstrates:

- Trust boundary identification
- Asset-driven threat modeling
- STRIDE classification
- Secure API design principles
- Security regression validation

---

This repository will evolve incrementally as additional security layers are introduced in later iterations.

