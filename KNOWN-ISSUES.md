# Known Issues

## Build Error: Resource file "**/*.resx" cannot be found

**Status**: Pre-existing issue (not related to CI/CD implementation)

**Description**:
When building the `Ligot.DbApi/Ligot.BackEnd.csproj` project with a clean build (no cached obj/bin directories), the following error occurs:

```
error MSB3552: Resource file "**/*.resx" cannot be found.
```

**Impact**:
- This error prevents clean builds in Release configuration
- The error occurs even before the CI/CD changes were made
- The error is masked when previous build outputs exist in obj/bin directories

**Root Cause**:
The MSBuild system is attempting to process a `**/*.resx` glob pattern, but this pattern is not explicitly defined in the project file. This suggests:
1. A corrupted build cache or NuGet package
2. An issue with .NET 10 SDK version 10.0.102
3. A missing or misconfigured build property

**Workaround**:
Currently, the builds work when obj/bin directories contain previous build outputs. This is not ideal for CI/CD pipelines which typically start with clean builds.

**Recommendation**:
This issue should be investigated and resolved separately from the CI/CD implementation. Possible solutions:
1. Update to a newer .NET SDK version if available
2. Add an explicit `<EnableDefaultEmbeddedResourceItems>false</EnableDefaultEmbeddedResourceItems>` to the project file
3. Investigate if a NuGet package is incorrectly adding the resx glob pattern

**CI/CD Impact**:
The GitHub Actions workflows will need to be tested with the actual Azure environment. If this build error persists in the GitHub Actions runners, one of the recommended solutions above should be applied before running the CI/CD pipelines.

**Related Files**:
- `Ligot.DbApi/Ligot.BackEnd.csproj`
- `.github/workflows/ci-build.yml`
- `.github/workflows/deploy-staging.yml`
- `.github/workflows/deploy-production.yml`

**Investigation Steps Taken**:
1. Verified the error exists on commit `db68a7a` (before CI/CD changes)
2. Confirmed no `.resx` files exist in the project
3. Confirmed no explicit `.resx` references in the `.csproj` file
4. Cleared NuGet caches - issue persists
5. Removed all obj/bin directories - issue persists
6. Tested with both Debug and Release configurations - error in both

**Next Steps**:
1. Test the CI/CD workflows in the actual Azure/GitHub Actions environment
2. If the error persists there, apply one of the recommended fixes
3. Document the solution once found

