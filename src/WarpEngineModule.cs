using UnityEngine;

namespace SurgeDriveMod
{
    public class WarpEngineModule : PartModule
    {
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Acceleration", guiUnits = " m/s²")]
        [UI_FloatRange(minValue = 0f, maxValue = 200f, stepIncrement = 1f, scene = UI_Scene.All)]
        public float maxWarpSpeed = 20f;

        [KSPField(guiActive = true, guiName = "Warp Throttle")]
        public string throttleDisplay = "0%";

        [KSPField(guiActive = true, guiName = "Vessel Accel")]
        public string vesselAccelDisplay = "0.00 g";

        [KSPField(guiActive = true, guiName = "EC Drain", guiUnits = " EC/s")]
        public string ecDrainDisplay = "0.0";

        [KSPField(isPersistant = true)]
        public bool isActive = true;

        public float actualEcRatio = 0f;

        // True only if this drive applied a SURGE delta-v in its latest physics update.
        [KSPField(guiActive = true, guiName = "SURGE State")]
        public string surgeStateDisplay = "Idle";

        [KSPEvent(guiActive = true, guiName = "Disable Warp Engine")]
        public void ToggleEngine()
        {
            isActive = !isActive;
            Events["ToggleEngine"].guiName = isActive ? "Disable Warp Engine" : "Enable Warp Engine";
        }

        [KSPAction("Activate Warp Engine")]
        public void ActivateAction(KSPActionParam _) { isActive = true; }

        [KSPAction("Deactivate Warp Engine")]
        public void DeactivateAction(KSPActionParam _) { isActive = false; }

        [KSPAction("Toggle Warp Engine")]
        public void ToggleAction(KSPActionParam _) { isActive = !isActive; }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            if (state != StartState.Editor)
            {
                part.force_activate();
                GameEvents.onFlightReady.Add(OnFlightReady);
            }
        }

        private void OnFlightReady()
        {
            GameEvents.onFlightReady.Remove(OnFlightReady);
            if (FlightInputHandler.state != null)
                FlightInputHandler.state.mainThrottle = 0f;
        }

        public void OnDestroy()
        {
            GameEvents.onFlightReady.Remove(OnFlightReady);
        }

        public override void OnUpdate()
        {
            if (!HighLogic.LoadedSceneIsFlight || vessel == null) return;

            float throttle = vessel.ctrlState.mainThrottle;
            throttleDisplay = Mathf.RoundToInt(throttle * 100) + "%";

            float totalCurrentAccel = 0f;
            float totalDecelAccel = 0f;
            foreach (Part p in vessel.parts)
            {
                WarpEngineModule warp = p.FindModuleImplementing<WarpEngineModule>();
                if (warp == null || !warp.isActive) continue;
                totalCurrentAccel += warp.maxWarpSpeed * throttle * warp.actualEcRatio;
                totalDecelAccel += warp.maxWarpSpeed;
            }
            vesselAccelDisplay = $"{totalCurrentAccel / 9.80665f:F2} g";

            if (IsPrimaryDrive())
                UpdateMarker(totalDecelAccel);
        }

        private bool IsPrimaryDrive()
        {
            foreach (Part p in vessel.parts)
            {
                var warp = p.FindModuleImplementing<WarpEngineModule>();
                if (warp != null) return warp == this;
            }
            return false;
        }

        private WarpMarkerRenderer GetOrCreateRenderer()
        {
            var r = vessel.gameObject.GetComponent<WarpMarkerRenderer>();
            if (r == null)
            {
                r = vessel.gameObject.AddComponent<WarpMarkerRenderer>();
                r.Init(vessel);
            }
            return r;
        }

        private void UpdateMarker(float totalDecelAccel)
        {
            var renderer = GetOrCreateRenderer();
            ITargetable target = vessel.targetObject;

            if (target == null || totalDecelAccel <= 0f || vessel.orbit == null)
            {
                renderer.hasValidMarker = false;
                return;
            }

            double a = totalDecelAccel;
            Vector3d vesselPos = vessel.GetWorldPos3D();
            Vector3d targetPos = (Vector3d)target.GetTransform().position;
            Vector3d relPos = vesselPos - targetPos;
            double D = relPos.magnitude;

            Vector3d relVel = vessel.GetObtVelocity() - target.GetObtVelocity();
            double vRel = relVel.magnitude;

            if (vRel < 1.0 || D < 1.0)
            {
                renderer.hasValidMarker = false;
                return;
            }

            // Positive closing rate means approaching target
            double closingRate = -Vector3d.Dot(relVel, relPos.normalized);
            if (closingRate <= 0.0)
            {
                renderer.hasValidMarker = false;
                return;
            }

            double dDecel = (vRel * vRel) / (2.0 * a);
            double timeToFlip = (D - dDecel) / closingRate;

            double now = Planetarium.GetUniversalTime();
            renderer.markerUT = timeToFlip > 0.0 ? now + timeToFlip : now;
            renderer.timeToMarker = timeToFlip;
            renderer.hasValidMarker = true;
        }

        public override void OnFixedUpdate()
        {
            if (!HighLogic.LoadedSceneIsFlight || vessel == null) return;

            float throttle = vessel.ctrlState.mainThrottle;
            if (!isActive || maxWarpSpeed <= 0f || throttle <= 0f)
            {
                ecDrainDisplay = "0.0";
                actualEcRatio = 0f;
                surgeStateDisplay = isActive ? "Idle" : "Disabled";
                return;
            }

            // Reject invalid physics steps before requesting power or modifying velocity.
            float dt = Time.fixedDeltaTime;
            if (dt <= 0f || float.IsNaN(dt) || float.IsInfinity(dt))
            {
                actualEcRatio = 0f;
                ecDrainDisplay = "0.0";
                surgeStateDisplay = "Invalid physics step";
                return;
            }

            double ecPerSecond = maxWarpSpeed * throttle;
            double ecRequired = ecPerSecond * dt;
            double ecReceived = part.RequestResource("ElectricCharge", ecRequired);

            if (double.IsNaN(ecReceived) || double.IsInfinity(ecReceived) || ecReceived <= 0.0)
            {
                isActive = false;
                actualEcRatio = 0f;
                ecDrainDisplay = "0.0";
                Events["ToggleEngine"].guiName = "Enable Warp Engine";
                surgeStateDisplay = "No EC / disabled";
                return;
            }

            // Resource providers may return partial EC. Never turn an anomalous
            // over-delivery into more acceleration than was requested.
            actualEcRatio = Mathf.Clamp01((float)(ecReceived / ecRequired));
            ecDrainDisplay = $"{ecPerSecond * actualEcRatio:F1}";
            if (actualEcRatio <= 0f)
            {
                surgeStateDisplay = "No EC";
                return;
            }

            float warpSpeed = maxWarpSpeed * throttle * actualEcRatio;
            Vector3 forward = part.transform.up;
            Vector3 deltaV = forward * warpSpeed * dt;
            vessel.ChangeWorldVelocity(deltaV);
            surgeStateDisplay = actualEcRatio < 1f ? "Power limited" : "Accelerating";
        }
    }
}
