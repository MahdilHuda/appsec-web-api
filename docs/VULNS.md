# Intentional Vulnerabilities

This project intentionally contained security vulnerabilities for Application Security training and demonstration purposes.

The vulnerable version must **never** be deployed to production.

The vulnerabilities were introduced and remediated as part of structured Application Security practice.

---

## Environment

- Local development only
- Base URL: `https://localhost:7096`
- Framework: ASP.NET Core (.NET)

---

# VULN 1 — Mass Assignment / Over-posting (Client-controlled Id)

## Where
- Endpoint: `POST /products`
- Component: `ProductsController` → `Create(...)`

## Description

The API originally accepted the `id` field directly from client input.  
This resulted in a trust boundary failure where untrusted HTTP input controlled a server-authoritative property.

The server did not enforce ownership of identifier generation.

## How to Exploit

An attacker could submit a custom `id` value in the request body.  
The server would persist this identifier without override or validation.

## Impact

- Loss of data integrity  
- Identifier collisions  
- Predictable or attacker-controlled identifiers  
- Elevated risk when combined with authorization or object reference checks  

---

## Status

✅**Fixed**

## Fix Summary

- Introduced a dedicated request DTO without `Id`
- Enforced server-generated identifiers
- Implemented thread-safe identifier generation
- Removed client influence over identity assignment

## Security Control

- Server authority over identifiers  
- Prevention of unauthorized state mutation  
- Proper trust boundary enforcement  

## Verification

Test Case: Client attempts to supply custom `id`

- Expected: Server ignores client-provided identifier
- Observed: `201 Created` with server-generated `id`
- Confirmed: `id` is not equal to attacker-supplied value

---

# VULN 2 — Missing Input Validation (Logic Abuse)

## Where
- Endpoint: `POST /products`
- Component: `ProductsController` → `Create(...)`

## Description

The API originally performed no server-side validation on product input fields such as `name` and `price`.

Invalid domain state was allowed to cross the HTTP trust boundary and persist in the application store.

## How to Exploit

An attacker could submit:

- Negative price values  
- Empty or whitespace-only names  

The API accepted and stored these invalid values.

## Impact

- Corrupted data integrity  
- Business logic abuse (e.g., negative pricing, manipulated totals)  
- Potential cascading failures in billing, reporting, or integrations  

---

## Status

✅**Fixed**

## Fix Summary

- Added server-side validation for:
  - Non-empty product name
  - Positive price values
- Invalid input now returns structured validation errors

## Security Control

- Boundary validation at HTTP ingress  
- Enforcement of domain invariants  
- Prevention of data poisoning  

## Verification

Test Case: Negative price submission

- Expected: `400 Bad Request`
- Observed: `400` with structured validation response

Test Case: Whitespace-only name

- Expected: `400 Bad Request`
- Observed: `400` with validation error

---

# VULN 3 — Unhandled Exception (500) / Potential Information Disclosure

## Where
- Endpoint: `GET /products/{id}`
- Component: `ProductsController` → `GetById(...)`

## Description

The endpoint originally failed to safely handle non-existent product identifiers.

Requests for missing resources triggered unhandled exceptions that propagated through the request pipeline.

## How to Exploit

An attacker could request a non-existent product ID repeatedly.

This caused the API to return `500 Internal Server Error`.

## Impact

- Application-level Denial of Service risk  
- Increased operational instability  
- Potential information disclosure in development or misconfigured environments  
- Poor error handling posture  

---

## Status

✅**Fixed**

## Fix Summary

- Implemented safe resource lookup
- Replaced exception-driven control flow with proper `404 Not Found`
- Added global exception handler returning standardized error responses

## Security Control

- Controlled error handling  
- Elimination of exception-driven 500 responses  
- Reduced disclosure surface  

## Verification

Test Case: Non-existent product ID

- Expected: `404 Not Found`
- Observed: `404`
- Confirmed: No 500 errors and no error detail leakage

---

# Summary

- Identification of trust boundary failures  
- Mapping vulnerabilities to system-level impact  
- Implementation of server authority and boundary validation  
- Controlled error handling  
- Security regression verification  

The API now enforces secure baseline behavior for the implemented endpoints.
