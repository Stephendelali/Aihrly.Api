# ANSWERS.md

## 1. Schema Question

```sql
-- applications
CREATE TABLE applications (
    id UUID PRIMARY KEY,
    job_id UUID NOT NULL REFERENCES jobs(id),
    candidate_name TEXT NOT NULL,
    candidate_email TEXT NOT NULL,
    cover_letter TEXT,
    stage TEXT NOT NULL DEFAULT 'applied',
    created_at TIMESTAMPTZ NOT NULL
);
CREATE INDEX idx_applications_job_id ON applications(job_id);
CREATE UNIQUE INDEX idx_applications_job_email ON applications(job_id, candidate_email);

-- application_notes
CREATE TABLE application_notes (
    id UUID PRIMARY KEY,
    application_id UUID NOT NULL REFERENCES applications(id),
    type TEXT NOT NULL,
    description TEXT NOT NULL,
    created_by UUID NOT NULL REFERENCES team_members(id),
    created_at TIMESTAMPTZ NOT NULL
);
CREATE INDEX idx_notes_application_id ON application_notes(application_id);

-- stage_history
CREATE TABLE stage_history (
    id UUID PRIMARY KEY,
    application_id UUID NOT NULL REFERENCES applications(id),
    from_stage TEXT NOT NULL,
    to_stage TEXT NOT NULL,
    changed_by UUID NOT NULL REFERENCES team_members(id),
    changed_at TIMESTAMPTZ NOT NULL,
    reason TEXT
);
CREATE INDEX idx_stage_history_application_id ON stage_history(application_id);

-- scores (one row per application per dimension)
CREATE TABLE scores (
    id UUID PRIMARY KEY,
    application_id UUID NOT NULL REFERENCES applications(id),
    dimension TEXT NOT NULL,
    score INT NOT NULL,
    comment TEXT,
    set_by UUID NOT NULL REFERENCES team_members(id),
    set_at TIMESTAMPTZ NOT NULL,
    UNIQUE(application_id, dimension)
);
CREATE INDEX idx_scores_application_id ON scores(application_id);
```

The most important indexes are on `application_id` in `application_notes`, `stage_history`, and `scores`. These make joins fast when fetching a full application profile — without them it would be a full table scan on every request.

The unique constraint on `(job_id, candidate_email)` enforces the duplicate-application rule at the database level, not just in application code.

**GET /api/applications/{id}** currently runs about 4 queries — one per related table. Using `.Include()` in EF Core would bring that down to a single joined query. Either way it's one round-trip to the database from the API's perspective.

---

## 2. Scoring Design Trade-off

**(a)** Three separate endpoints make sense here because each dimension means something different — interview scores might only be submitted by the interviewer, assessment scores could come from a different tool entirely. Keeping them separate makes the API explicit and easier to add per-dimension validation or permissions later. A single generic endpoint would make more sense if all three scores are always submitted together in one form, or if the number of dimensions is expected to grow and you don't want a new route every time.

**(b)** If score history was required, I'd drop the unique constraint and switch from overwrite to append — each PUT inserts a new row instead of replacing the old one. The current score would just be the latest row by `set_at` per dimension. The endpoint URLs and request bodies wouldn't change at all, just the write logic and how reads select the active value.

---

## 3. Debugging Question

A recruiter says: "I moved a candidate to Interview yesterday and today it still shows Screening."

1. Check `stage_history` for that application first — if a row exists with `to_stage = 'interview'` the write went through and the problem is on the read side
2. If no row exists, the request either failed or never reached the server — check server logs around the reported time
3. Look for any 4xx or 5xx errors on `PATCH /api/applications/{id}/stage` in the logs
4. Ask the recruiter whether they saw a success response — they may have navigated away before the request completed
5. If the write succeeded, check whether the frontend is caching the application state and not refetching after the PATCH
6. Check `stage_history` for any subsequent change — another team member may have moved it back
7. If the history shows the change but `applications.stage` doesn't match, there may be a transaction bug where `stage_history` was written but the application update wasn't committed — likely a missing `await` on `SaveChangesAsync`

---

## 4. Honest Self-Assessment

- **C#**: 3/5 — Comfortable building APIs with ASP.NET Core and common patterns, still growing in more advanced areas.
- **SQL**: 4/5 — Can design schemas, write queries, and think through indexing and constraints; use EF Core day to day but confident dropping into raw SQL when needed
- **Git**: 4/5 — Use it daily for branching, merging, and resolving conflicts; still improving on rebasing and more advanced workflows
- **REST API design**: 3/5 — Comfortable with resource modeling, HTTP semantics, status codes, and consistent error handling, but still improving with more complex API patterns and edge cases.
- **Writing tests**: 3/5 — Can write solid unit and integration tests, still working on coverage strategy and cleaner mocking patterns
