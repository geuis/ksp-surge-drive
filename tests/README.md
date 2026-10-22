# SURGE physics regression tests

The `SurgePhysicsMath.Tests` executable uses .NET 8, no KSP DLLs and no test framework/NuGet packages. Its source-linked calculations also compile in the mod's .NET Framework 4.7.2 project.

Run from the repository root:

```shell
dotnet run --project tests/SurgePhysicsMath.Tests/SurgePhysicsMath.Tests.csproj
```

The test covers zero throttle, zero/partial/excess EC, invalid timesteps and **arithmetic** for 1/2/4/8 identically oriented drives. These checks **do not** run KSP's `RequestResource`, `ChangeWorldVelocity`, Unity transforms or KSP physics. They cannot verify or resolve Issue #2's reported ~30g limit. In-game measurements are still essential.

Changes in this PR do not alter runtime propulsion control or integrate the helper into the existing `WarpEngineModule`. A subsequent targeted refactor may use it after checking compatibility and comparing in-game behavior.
