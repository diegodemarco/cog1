# Consolidated Validation Checklists (Code)

## General
- [ ] Names descriptive; magic values extracted
- [ ] Guard clauses; errors actionable
- [ ] Inputs validated; outputs sanitized
- [ ] Tests added/updated; deterministic
- [ ] Logs/metrics appropriate; no secrets logged
- [ ] Dead code removed; deps pinned/scanned

## TypeScript
- [ ] Public APIs typed; no `any`
- [ ] Type narrowing/guards used
- [ ] Async handled; no floating promises
- [ ] ESLint/Prettier clean

## React
- [ ] Props/state typed
- [ ] Effects dependencies correct; cleanup present
- [ ] Accessibility considered; behavior-focused tests

## C#
- [ ] Nullable enabled; null-handling explicit
- [ ] DI used; no static singletons
- [ ] Exceptions specific and meaningful

## ASP.NET Core Web API
- [ ] Controllers thin; services handle logic
- [ ] DTO validation/mapping explicit
- [ ] AuthZ policies enforced; health checks

## Java
- [ ] Single responsibility; composition preferred
- [ ] Optional used thoughtfully; null checks explicit
- [ ] Tests cover logic/integration

## Python
- [ ] Black/isort/mypy clean (where applicable)
- [ ] Specific exceptions; no bare `except`

## Terraform
- [ ] Modules small and reusable
- [ ] CI validate/plan; remote state locked
- [ ] Secrets managed securely

## Git/Jira
- [ ] Branch includes ticket key; conventional commits
- [ ] PR links ticket with test plan; CI posts status
