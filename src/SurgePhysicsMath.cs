using System;

namespace SurgeDriveMod
{
    // Pure, KSP-independent scalar calculations. The existing PartModule
    // remains responsible for EC acquisition and applying vessel velocity.
    public static class SurgePhysicsMath
    {
        public static double RequestedEcPerSecond(double accelerationMps2, double throttle)
        {
            if (!IsFinite(accelerationMps2) || !IsFinite(throttle)) return 0d;
            return Math.Max(0d, accelerationMps2) * Clamp01(throttle);
        }

        public static double PowerRatio(double requestedEc, double receivedEc)
        {
            if (!IsFinite(requestedEc) || !IsFinite(receivedEc) ||
                requestedEc <= 0d || receivedEc <= 0d) return 0d;
            return Clamp01(receivedEc / requestedEc);
        }

        public static double EffectiveAcceleration(double accelerationMps2, double throttle, double powerRatio)
        {
            if (!IsFinite(accelerationMps2) || !IsFinite(throttle) || !IsFinite(powerRatio))
                return 0d;
            return Math.Max(0d, accelerationMps2) * Clamp01(throttle) * Clamp01(powerRatio);
        }

        public static double DeltaV(double accelerationMps2, double throttle,
            double powerRatio, double fixedDeltaTime)
        {
            if (!IsFinite(fixedDeltaTime) || fixedDeltaTime <= 0d) return 0d;
            return EffectiveAcceleration(accelerationMps2, throttle, powerRatio) * fixedDeltaTime;
        }

        public static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

        private static double Clamp01(double value)
        {
            return Math.Max(0d, Math.Min(1d, value));
        }
    }
}
