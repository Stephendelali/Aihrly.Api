# Aihrly API

A backend API for an Applicant Tracking System (ATS) built with ASP.NET Core and PostgreSQL. This is the team-side pipeline — the part recruiters and hiring managers use to move candidates through stages, leave notes, and score applicants.

---

## Tech Stack

- .NET 8 / ASP.NET Core Web API
- PostgreSQL via EF Core (Npgsql)
- xUnit for testing

---

## Running Locally

### Prerequisites

- .NET 8 SDK
- PostgreSQL running locally

### Setup

1. Clone the repo
2. Create a `.env` file in the `Aihrly.Api` folder:

```env
DB_HOST=localhost
DB_PORT=5432
DB_NAME=aihrly
DB_USER=postgres
DB_PASSWORD=yourpassword
```

3. Apply migrations:

```bash
cd Aihrly.Api
dotnet ef database update
```

4. Run the API:

```bash
dotnet run
```

The API will be available at `https://localhost:7xxx` and Swagger UI at `/swagger`.

---

## Running the Tests

From the root of the repo:

```bash
dotnet test
```

Tests use an in-memory database so no PostgreSQL setup is needed to run them.

---

## Seeded Team Members

Three team members are seeded automatically on startup:

| Name | Email | Role | ID |
|------|-------|------|----|
| Alice Mensah | alice@aihrly.com | recruiter | `11111111-1111-1111-1111-111111111111` |
| Bob Asante | bob@aihrly.com | hiring_manager | `22222222-2222-2222-2222-222222222222` |
| Clara Owusu | clara@aihrly.com | recruiter | `33333333-3333-3333-3333-333333333333` |

Use any of these IDs in the `X-Team-Member-Id` header when hitting endpoints that require it.

---

## API Overview

### Jobs
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/jobs` | Create a job |
| GET | `/api/jobs` | List jobs (filter by `?status=open`, paginate with `?page=1&pageSize=20`) |
| GET | `/api/jobs/{id}` | Get a single job |

### Applications
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/jobs/{jobId}/applications` | Submit an application |
| GET | `/api/jobs/{jobId}/applications` | List applications for a job (filter by `?stage=screening`) |
| GET | `/api/applications/{id}` | Full applicant profile |
| PATCH | `/api/applications/{id}/stage` | Move to a new stage (requires `X-Team-Member-Id`) |

### Notes
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/applications/{id}/notes` | Add a note (requires `X-Team-Member-Id`) |
| GET | `/api/applications/{id}/notes` | List notes, newest first |

### Scores
| Method | Endpoint | Description |
|--------|----------|-------------|
| PUT | `/api/applications/{id}/scores/culture-fit` | Set culture fit score 1–5 |
| PUT | `/api/applications/{id}/scores/interview` | Set interview score 1–5 |
| PUT | `/api/applications/{id}/scores/assessment` | Set assessment score 1–5 |

---

## Pipeline Stages
