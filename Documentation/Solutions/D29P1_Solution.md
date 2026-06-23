# D29P1 Solution

## Commit Log

Repository: **AryanB-Capstone-FlightDex**
Branch: `main`

| Commit | Author | Date | Message |
| --- | --- | --- | --- |
| `aba5939` | Aryan Bhalerao | 2026-06-23 23:14:39 +0530 | chore(docker): add docker-compose for the full stack |
| `2ccb35b` | Aryan Bhalerao | 2026-06-23 23:14:39 +0530 | chore(docker): containerize the Angular frontend |
| `669fb86` | Aryan Bhalerao | 2026-06-23 23:14:29 +0530 | chore(docker): containerize the backend API |
| `d8bd8ef` | Aryan Bhalerao | 2026-06-23 23:14:13 +0530 | feat(UI): add ticket booking & my-tickets views |
| `6760d1e` | Aryan Bhalerao | 2026-06-23 23:14:12 +0530 | feat(UI): add authentication flow |
| `482fc6b` | Aryan Bhalerao | 2026-06-23 23:14:01 +0530 | feat(API): implement ticket booking & cancellation |
| `f164ec4` | Aryan Bhalerao | 2026-06-23 23:13:47 +0530 | feat(API): implement user auth with JWT & PBKDF2 hashing |
| `ab1c172` | Aryan Bhalerao | 2026-06-23 23:13:36 +0530 | feat(API): add booking domain — Ticket & User |
| `cff4cd4` | Aryan Bhalerao | 2026-06-23 23:12:59 +0530 | feat(UI): scaffold Angular client with flight timetable view |
| `dba3dd7` | Aryan Bhalerao | 2026-06-23 23:12:39 +0530 | feat(API): add Flights infrastructure, CSV seeder & host wiring |
| `c78eff2` | Aryan Bhalerao | 2026-06-23 23:12:28 +0530 | feat(API): model the flight timetable domain & queries |
| `eb1ff75` | Aryan Bhalerao | 2026-06-23 23:12:20 +0530 | feat(API): add CQRS shared kernel |
| `9b8a7fc` | Aryan Bhalerao | 2026-06-23 23:12:13 +0530 | chore: add .gitignore for build output, local exports & IDE files |
| `be694db` | Aryan Bhalerao | 2026-06-23 22:41:24 +0530 | Reinitialized main branch |

---

## Curl Test

Every endpoint exercised against the running stack (`docker compose up`), API at
`http://localhost:5162`. Each block shows the request and the observed response with
its HTTP status. The full flow — register → login → browse flights → book → list →
cancel — plus the error paths (400 / 401 / 404 / 409) is covered.

### Auth — `POST /auth/register`, `POST /auth/login`

**Register a new account (`200 OK`)** — returns a bearer token and the user profile:

```bash
curl -i -X POST http://localhost:5162/auth/register \
  -H 'Content-Type: application/json' \
  -d '{"email":"curldoc@flightdex.dev","firstName":"Ada","lastName":"Lovelace","age":36,"isGovernmentOfficial":false,"isLawEnforcementOrMilitary":false,"password":"Passw0rd!23"}'
```
```jsonc
// HTTP 200
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...bQLOKufcWJXPEceQHU2dyjwv5pKlsqi7Rn2dSPtYILM",
  "expiresAtUtc": "2026-06-24T18:31:47.4280286Z",
  "user": {
    "id": 4, "email": "curldoc@flightdex.dev", "firstName": "Ada", "lastName": "Lovelace",
    "age": 36, "isGovernmentOfficial": false, "isLawEnforcementOrMilitary": false
  }
}
```

**Duplicate email (`409 Conflict`):**

```bash
curl -i -X POST http://localhost:5162/auth/register \
  -H 'Content-Type: application/json' \
  -d '{"email":"curldoc@flightdex.dev","firstName":"Ada","lastName":"Lovelace","age":36,"isGovernmentOfficial":false,"isLawEnforcementOrMilitary":false,"password":"Passw0rd!23"}'
```
```jsonc
// HTTP 409
{ "title": "Conflict", "status": 409, "detail": "An account already exists for 'curldoc@flightdex.dev'." }
```

**Missing required fields (`400 Bad Request`):**

```bash
curl -i -X POST http://localhost:5162/auth/register \
  -H 'Content-Type: application/json' \
  -d '{"email":"","password":"","firstName":"","lastName":"","age":0,"isGovernmentOfficial":false,"isLawEnforcementOrMilitary":false}'
```
```jsonc
// HTTP 400
{ "title": "Bad Request", "status": 400, "detail": "Email and password are required." }
```

**Log in (`200 OK`)** — capture the token for the ticket calls below:

```bash
TOKEN=$(curl -s -X POST http://localhost:5162/auth/login \
  -H 'Content-Type: application/json' \
  -d '{"email":"curldoc@flightdex.dev","password":"Passw0rd!23"}' \
  | sed -n 's/.*"token":"\([^"]*\)".*/\1/p')
```
```jsonc
// HTTP 200
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...-sKODlDPt9q3bc1QyeXfY2ptXDo0YjtdqhD3VtYhOnI",
  "expiresAtUtc": "2026-06-24T18:31:47.6148331Z",
  "user": { "id": 4, "email": "curldoc@flightdex.dev", "firstName": "Ada", "lastName": "Lovelace", "age": 36, "isGovernmentOfficial": false, "isLawEnforcementOrMilitary": false }
}
```

**Wrong password (`401 Unauthorized`):**

```bash
curl -i -X POST http://localhost:5162/auth/login \
  -H 'Content-Type: application/json' \
  -d '{"email":"curldoc@flightdex.dev","password":"wrong"}'
```
```jsonc
// HTTP 401
{ "title": "Unauthorized", "status": 401, "detail": "Invalid email or password." }
```

### Flights — `GET /flight`, `GET /flight/{flightCode}` (public)

**Departures from an airport (`200 OK`)** — paged, sorted by time:

```bash
curl -i 'http://localhost:5162/flight?at=BOM&pageSize=3'
```
```jsonc
// HTTP 200
{
  "items": [
    { "flightCode": "6E91",   "time": "00:15", "airline": "IndiGo",    "city": "Jeddah",    "code": "JED", "airport": "King Abdulaziz International Airport",            "direction": "Departure" },
    { "flightCode": "6E6187", "time": "00:30", "airline": "IndiGo",    "city": "Hyderabad", "code": "HYD", "airport": "Rajiv Gandhi International Airport",              "direction": "Departure" },
    { "flightCode": "QP1135", "time": "00:35", "airline": "Akasa Air", "city": "Kolkata",   "code": "CCU", "airport": "Kolkata Netaji Subhas Chandra Bose Intl Airport", "direction": "Departure" }
  ],
  "page": 1, "pageSize": 3, "totalCount": 402, "totalPages": 134, "hasPrevious": false, "hasNext": true
}
```

**Departures filtered by destination (`?to=`):**

```bash
curl -i 'http://localhost:5162/flight?at=BLR&to=DEL&pageSize=3'
```
```jsonc
// HTTP 200 — 36 BLR→DEL departures; first is AI2758 @ 00:15
{ "items": [ { "flightCode": "AI2758", "time": "00:15", "airline": "Air India", "city": "Delhi", "code": "DEL", "direction": "Departure" }, "...": "..." ], "totalCount": 36 }
```

**Arrivals at an airport (presence of `from` selects arrivals):**

```bash
curl -i 'http://localhost:5162/flight?at=BOM&from&pageSize=3'
```
```jsonc
// HTTP 200 — first arrival KL877 (Amsterdam) @ 00:01
{ "items": [ { "flightCode": "KL877", "time": "00:01", "airline": "KLM", "city": "Amsterdam", "code": "AMS", "direction": "Arrival" }, "...": "..." ], "totalCount": 401 }
```

**Departure time window (`HHMM`):**

```bash
curl -i 'http://localhost:5162/flight?at=BLR&deptTime_After=0800&deptTime_Before=1200&pageSize=3'
```
```jsonc
// HTTP 200 — 74 departures between 08:00 and 12:00; first is S5183 @ 08:00
{ "items": [ { "flightCode": "S5183", "time": "08:00", "airline": "Star Air", "city": "Nanded", "code": "NDC", "direction": "Departure" }, "...": "..." ], "totalCount": 74 }
```

**Unserved airport (`400 Bad Request`):**

```bash
curl -i 'http://localhost:5162/flight?at=XXX'
```
```jsonc
// HTTP 400
{ "title": "Bad Request", "status": 400, "detail": "'at' must be one of BLR, BOM or PNQ." }
```

**Flight detail by code (`200 OK`):**

```bash
curl -i http://localhost:5162/flight/6E91
```
```jsonc
// HTTP 200
{
  "flightCode": "6E91", "airport": "BOM", "direction": "Departure", "time": "00:15",
  "airlineCode": "6E", "airline": "IndiGo",
  "counterpartAirport": "King Abdulaziz International Airport", "counterpartCode": "JED",
  "counterpartCity": "Jeddah", "duration": "5h 25m"
}
```

**Unknown flight code (`404 Not Found`):**

```bash
curl -i http://localhost:5162/flight/NOPE99
```
```jsonc
// HTTP 404
{ "title": "Not Found", "status": 404 }
```

### Tickets — `POST /ticket`, `GET /ticket`, `DELETE /ticket/{id}` (auth required)

**No bearer token (`401 Unauthorized`):**

```bash
curl -i http://localhost:5162/ticket
```
```text
HTTP 401   (empty body — JWT bearer challenge)
```

**Book a ticket (`201 Created`)** — passenger name/age come from the token, not the body:

```bash
curl -i -X POST http://localhost:5162/ticket \
  -H "Authorization: Bearer $TOKEN" \
  -H 'Content-Type: application/json' \
  -d '{"date":"2026-07-15","time":"09:30","origin":{"code":"BOM","airport":"Chhatrapati Shivaji Maharaj International Airport","city":"Mumbai"},"destination":{"code":"DEL","airport":"Indira Gandhi International Airport","city":"Delhi"}}'
```
```jsonc
// HTTP 201
{
  "ticketId": 7, "date": "2026-07-15", "time": "09:30",
  "originCode": "BOM", "originAirport": "Chhatrapati Shivaji Maharaj International Airport", "originCity": "Mumbai",
  "destinationCode": "DEL", "destinationAirport": "Indira Gandhi International Airport", "destinationCity": "Delhi",
  "firstName": "Ada", "lastName": "Lovelace", "age": 36
}
```

**List my tickets (`200 OK`):**

```bash
curl -i http://localhost:5162/ticket -H "Authorization: Bearer $TOKEN"
```
```jsonc
// HTTP 200
[ { "ticketId": 7, "date": "2026-07-15", "time": "09:30", "originCode": "BOM", "destinationCode": "DEL", "firstName": "Ada", "lastName": "Lovelace", "age": 36 } ]
```

**Cancel my ticket (`204 No Content`):**

```bash
curl -i -X DELETE http://localhost:5162/ticket/7 -H "Authorization: Bearer $TOKEN"
```
```text
HTTP 204   (empty body)
```

**Cancel a non-existent ticket (`404 Not Found`):**

```bash
curl -i -X DELETE http://localhost:5162/ticket/999999 -H "Authorization: Bearer $TOKEN"
```
```jsonc
// HTTP 404
{ "title": "Not Found", "status": 404 }
```

| # | Endpoint | Case | Status |
| --- | --- | --- | --- |
| 1 | `POST /auth/register` | new account | `200` |
| 2 | `POST /auth/register` | duplicate email | `409` |
| 3 | `POST /auth/register` | missing fields | `400` |
| 4 | `POST /auth/login` | valid credentials | `200` |
| 5 | `POST /auth/login` | wrong password | `401` |
| 6 | `GET /flight?at=BOM` | departures | `200` |
| 7 | `GET /flight?at=BLR&to=DEL` | departures to dest | `200` |
| 8 | `GET /flight?at=BOM&from` | arrivals | `200` |
| 9 | `GET /flight?...deptTime_After/Before` | time window | `200` |
| 10 | `GET /flight?at=XXX` | unserved airport | `400` |
| 11 | `GET /flight/6E91` | flight by code | `200` |
| 12 | `GET /flight/NOPE99` | unknown code | `404` |
| 13 | `GET /ticket` | no token | `401` |
| 14 | `POST /ticket` | book | `201` |
| 15 | `GET /ticket` | list mine | `200` |
| 16 | `DELETE /ticket/7` | cancel | `204` |
| 17 | `DELETE /ticket/999999` | unknown ticket | `404` |

---

## UI Snapshots

- **Register**

  ![Register](../Snapshots/Register.png)

- **Register — confirm password validation**

  ![Register confirm password](../Snapshots/Register_ConfirmPassword.png)

- **Login**

  ![Login](../Snapshots/Login.png)

- **Flight timetable**

  ![Timetable](../Snapshots/Timetable.png)

- **Book a ticket**

  ![Book Tickets](../Snapshots/BookTickets.png)

- **My tickets**

  ![My Tickets](../Snapshots/MyTickets.png)

---

## UI Walkthrough

End-to-end screen recording of the app (register → login → browse the timetable → book a
ticket → view & cancel under *My Tickets*):

📹 [UIWalkThrough.mp4](../UIWalkThrough.mp4)
