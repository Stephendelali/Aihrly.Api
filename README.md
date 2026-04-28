# Aihrly API

A backend API for an Applicant Tracking System (ATS). Recruiters and hiring managers use it to move candidates through hiring stages, leave notes, and score applicants.

**Stack:** .NET 8, ASP.NET Core, PostgreSQL, EF Core, xUnit

---

## Running Locally

1. Create a `.env` file inside the `Aihrly.Api` folder:

```env
DB_HOST=localhost
DB_PORT=5432
DB_NAME=aihrly
DB_USER=postgres
DB_PASSWORD=yourpassword
```

2. Apply migrations and run:

```bash
cd Aihrly.Api
dotnet ef database update
dotnet run
```

Swagger is available at `/swagger`.

---

## Running Tests

```bash
dotnet test
```

No database needed — tests use an in-memory provider.

---

## Seeded Team Members

These are created automatically on startup. Use any ID as the `X-Team-Member-Id` header.

| Name | Role | ID |
|------|------|----|
| Alice Mensah | recruiter | `11111111-1111-1111-1111-111111111111` |
| Bob Asante | hiring_manager | `22222222-2222-2222-2222-222222222222` |
| Clara Owusu | recruiter | `33333333-3333-3333-3333-333333333333` |

---

## Endpoints

**Jobs**
- `POST /api/jobs` — create a job
- `GET /api/jobs` — list jobs (`?status=open`, `?page=1&pageSize=20`)
- `GET /api/jobs/{id}` — get a job

**Applications**
- `POST /api/jobs/{jobId}/applications` — submit an application
- `GET /api/jobs/{jobId}/applications` — list applications (`?stage=screening`)
- `GET /api/applications/{id}` — full applicant profile
- `PATCH /api/applications/{id}/stage` — move stage (requires `X-Team-Member-Id`)

**Notes**
- `POST /api/applications/{id}/notes` — add a note (requires `X-Team-Member-Id`)
- `GET /api/applications/{id}/notes` — list notes, newest first

**Scores** (all require `X-Team-Member-Id`)
- `PUT /api/applications/{id}/scores/culture-fit`
- `PUT /api/applications/{id}/scores/interview`
- `PUT /api/applications/{id}/scores/assessment`

---

## Pipeline Stages

`applied → screening → interview → offer → hired / rejected`

Invalid transitions return `400`.

---

## Part 2 — Background Notifications

When an application moves to `hired` or `rejected`, a notification is dispatched asynchronously via a .NET `BackgroundService` and an in-memory channel. The PATCH endpoint returns immediately while the worker logs the event and inserts a row into the `notifications` table. I chose `BackgroundService` over Hangfire to avoid extra infrastructure — the trade-off is notifications are lost on crash. For production I'd use Hangfire or a message queue.

---

## Assumptions

- No auth — team members are identified by `X-Team-Member-Id` and validated against the DB
- Scores are overwrite — submitting twice replaces the previous value
- Notifications simulate sending an email, no actual email is sent

---

## What I'd Improve With More Time

- Use `.Include()` on the profile endpoint to reduce it to one DB query
- Add pagination to the applications list
- Add more test coverage for scores and edge cases
- Docker Compose file for one-command setup
