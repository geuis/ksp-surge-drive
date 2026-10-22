using SurgeDriveMod;

int assertions = 0;
void Near(string name, double expected, double actual)
{
    assertions++;
    if (Math.Abs(expected - actual) > 1e-9)
        throw new Exception($"{name}: expected {expected}, got {actual}");
}

Near("zero throttle", 0, SurgePhysicsMath.DeltaV(200, 0, 1, 0.02));
Near("single-drive nominal", 4, SurgePhysicsMath.DeltaV(200, 1, 1, 0.02));
Near("50 percent throttle", 2, SurgePhysicsMath.DeltaV(200, 0.5, 1, 0.02));
Near("partial power", 1, SurgePhysicsMath.DeltaV(200, 1, 0.25, 0.02));
Near("zero power", 0, SurgePhysicsMath.DeltaV(200, 1, 0, 0.02));
Near("over-supplied power clamps", 4, SurgePhysicsMath.DeltaV(200, 1, 2, 0.02));
Near("negative timestep", 0, SurgePhysicsMath.DeltaV(200, 1, 1, -1));
Near("NaN timestep", 0, SurgePhysicsMath.DeltaV(200, 1, 1, double.NaN));
Near("EC rate", 200, SurgePhysicsMath.RequestedEcPerSecond(200, 1));
Near("EC at half throttle", 100, SurgePhysicsMath.RequestedEcPerSecond(200, 0.5));
Near("EC zero demand", 0, SurgePhysicsMath.PowerRatio(0, 10));
Near("EC partial", 0.25, SurgePhysicsMath.PowerRatio(4, 1));
Near("EC capped", 1, SurgePhysicsMath.PowerRatio(4, 6));
Near("EC negative delivery", 0, SurgePhysicsMath.PowerRatio(4, -1));
Near("EC NaN delivery", 0, SurgePhysicsMath.PowerRatio(4, double.NaN));

foreach (int count in new[] { 1, 2, 4, 8 })
{
    // For aligned, independently powered drives, increments sum linearly.
    double perDrive = SurgePhysicsMath.DeltaV(200, 1, 1, 0.02);
    Near($"{count} aligned drives", 4 * count, perDrive * count);
}

Console.WriteLine($"PASS: {assertions} SURGE scalar math checks");
