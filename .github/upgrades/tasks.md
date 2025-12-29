# AspireApp1 .NET 10.0 Upgrade Tasks

## Overview

This document tracks the execution of the AspireApp1 solution upgrade from .NET 9.0 to .NET 10.0. All 6 projects will be upgraded simultaneously in a single atomic operation, followed by testing and validation.

**Progress**: 0/3 tasks complete (0%) ![0%](https://progress-bar.xyz/0)

---

## Tasks

### [▶] TASK-001: Verify prerequisites
**References**: Plan §Phase 0

- [▶] (1) Verify .NET 10.0 SDK installed per Plan §Prerequisites
- [ ] (2) SDK version meets minimum requirements (**Verify**)

---

### [ ] TASK-002: Atomic framework and dependency upgrade
**References**: Plan §Phase 1, Plan §Package Update Reference, Plan §Breaking Changes Catalog

- [ ] (1) Update TargetFramework to net10.0 in all 6 project files per Plan §Phase 1
- [ ] (2) All project files updated (**Verify**)
- [ ] (3) Update all package references per Plan §Package Update Reference (focus: Aspire 9.0→13.1, EF Core 8.0→10.0, ASP.NET Core Auth 8.0→10.0, OpenTelemetry 1.9→1.14)
- [ ] (4) All package references updated (**Verify**)
- [ ] (5) Apply TimeSpan.FromSeconds source incompatibility fix per Plan §Breaking Changes Catalog (Tests\WebTests.cs line 21: change to TimeSpan.FromSeconds(30.0))
- [ ] (6) Source incompatibility fix applied (**Verify**)
- [ ] (7) Restore all dependencies
- [ ] (8) Dependencies restored successfully (**Verify**)
- [ ] (9) Build solution and fix all compilation errors per Plan §Breaking Changes Catalog
- [ ] (10) Solution builds with 0 errors (**Verify**)
- [ ] (11) Commit changes with message: "TASK-002: Atomic upgrade AspireApp1 to .NET 10.0"

---

### [ ] TASK-003: Run full test suite and validate upgrade
**References**: Plan §Phase 2 Testing, Plan §Breaking Changes Catalog

- [ ] (1) Run tests in AspireApp1.Tests project
- [ ] (2) Fix any test failures per Plan §Breaking Changes Catalog (reference behavioral changes: System.Uri, HttpContent, ExceptionHandler)
- [ ] (3) Re-run tests after fixes
- [ ] (4) All tests pass with 0 failures (**Verify**)
- [ ] (5) Commit test fixes with message: "TASK-003: Complete .NET 10.0 upgrade testing and validation"

---
