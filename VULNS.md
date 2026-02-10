# Intentional Vulnerabilities

This project intentionally contains security vulnerabilities for Application Security training and demonstration purposes.
This version must **never** be deployed to production.

## Environment
- Local development only
- Base URL: https://localhost:7096
- Framework: ASP.NET Core (.NET)

---

## VULN 1 — Mass Assignment / Over-posting (Client-controlled Id)

**Where**
- Endpoint: `POST /products`
- Code: `ProductsController.cs` — `Create(...)`

**Description**
The API accepts the `id` field directly from client input. The server does not enforce ownership of identifier generation.

**How to exploit**
An attacker can submit a custom `id` value in the request body, which is stored without validation or override.

**Impact**
- Loss of data integrity
- Identifier collisions
- Predictable or attacker-controlled identifiers
- Increased risk when combined with authorization or object reference checks

**Proof**  
Request:
```http
POST /products
Content-Type: application/json

{
  "id": 999,
  "name": "Injected Id",
  "price": 10.00
} 
```
---
## VULN 2 — Missing Input Validation (Logic Abuse)

**Where**
- Endpoint: `POST /products`
- Code: `ProductsController.cs` — `Create(...)`

**Description**
The API does not apply any server-side validation to product input fields such as `name` or `price`.

**How to exploit**
An attacker can submit logically invalid values, including negative prices or empty product names.

**Impact**
- Corrupted data integrity
- Business logic abuse (e.g., negative pricing, free or refunded products)
- Potential cascading failures in downstream systems (billing, reporting, integrations)

**Proof**  
Negative price:
```http
POST /products
Content-Type: application/json

{
  "name": "Free money",
  "price": -999.99
}
```
Empty name:
```http
{
  "name": "",
  "price": 12.34
}
```
Expected result:
- Requests are accepted
- Invalid data is persisted and visible via `GET /products`
---
## VULN 3 — Unhandled Exception (500) / Potential Information Disclosure

**Where**
- Endpoint: `GET /products/{id}`
- Code: `ProductsController.cs` — `GetById(...)`

**Description**
The endpoint does not safely handle requests for non-existing resources.

**How to exploit**
Requesting a product ID that does not exist triggers an unhandled exception.

**Impact**
- Repeated 500 errors (reliability and DoS risk)
- Potential information disclosure (stack traces in development environments, verbose logs)
- Poor error handling patterns that complicate future hardening

**Proof**  
```http
GET /products/123456
```
Expected result:
- 500 Internal Server Error
- Additional error details may be exposed depending on environment configuration
