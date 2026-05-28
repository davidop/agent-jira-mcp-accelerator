# Release Checklist v1.0

## Go/No-Go Gates

- [ ] CI on main is green with xUnit and Playwright smoke stages.
- [ ] `docs/testing/coverage-by-layer.mmd` and `docs/testing/ci-execution-flow.mmd` are current.
- [ ] Deploy workflow can provision, publish images to ACR, and run runtime deployment stages.
- [ ] Runtime URL verification step in deploy workflow reports non-empty URLs.
- [ ] README and demo script reflect shipped scope and current limitations.
- [ ] No secrets are committed in repository history for this release scope.

## Evidence Pointers

- CI workflow: `.github/workflows/ci.yml`
- Deploy workflow: `.github/workflows/deploy.yml`
- Testing docs: `docs/testing/`
- Verification baseline: `docs/gsd/05-verification-checklist.md`
- Release notes: `docs/gsd/07-release-notes-v1.0.md`

## Sign-Off

- Product/Presales:
- Engineering:
- Platform/Security:
- Release date:
- Tag:
