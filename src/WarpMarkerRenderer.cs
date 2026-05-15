using UnityEngine;

namespace SurgeDriveMod
{
    public class WarpMarkerRenderer : MonoBehaviour
    {
        public double markerUT = -1.0;
        public bool hasValidMarker = false;
        public double timeToMarker = 0.0;

        private Vessel _vessel;
        private Texture2D _coreTex;
        private Texture2D _glowTex;

        private static readonly Color CoreColor = new Color(0.72f, 0.1f, 1f, 1f);
        private static readonly Color GlowColor = new Color(0.65f, 0.1f, 1f, 0.2f);

        public void Init(Vessel vessel)
        {
            _vessel = vessel;
            _coreTex = MakeCircle(16, CoreColor);
            _glowTex = MakeCircle(64, GlowColor);
        }

        private static Texture2D MakeCircle(int size, Color color)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float c = (size - 1) / 2f;
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    float d = Mathf.Sqrt((x - c) * (x - c) + (y - c) * (y - c));
                    float alpha = Mathf.Clamp01(1f - d / c);
                    tex.SetPixel(x, y, new Color(color.r, color.g, color.b, color.a * alpha));
                }
            tex.Apply();
            return tex;
        }

        private void OnGUI()
        {
            if (!MapView.MapIsEnabled || !hasValidMarker || _vessel == null || _vessel.orbit == null) return;

            Vector3d worldPos = _vessel.orbit.getPositionAtUT(markerUT);
            Vector3d scaledPos = ScaledSpace.LocalToScaledSpace(worldPos);
            Vector3 screenPos = PlanetariumCamera.Camera.WorldToScreenPoint((Vector3)scaledPos);

            if (screenPos.z <= 0f) return;

            float sx = screenPos.x;
            float sy = Screen.height - screenPos.y;

            Color prev = GUI.color;

            GUI.color = Color.white;
            float glowSize = 40f;
            GUI.DrawTexture(new Rect(sx - glowSize * 0.5f, sy - glowSize * 0.5f, glowSize, glowSize), _glowTex);

            float coreSize = 10f;
            GUI.DrawTexture(new Rect(sx - coreSize * 0.5f, sy - coreSize * 0.5f, coreSize, coreSize), _coreTex);

            GUI.color = new Color(0.85f, 0.6f, 1f, 1f);
            GUI.Label(new Rect(sx + 12f, sy - 9f, 200f, 20f), FormatCountdown(timeToMarker));

            GUI.color = prev;
        }

        private static string FormatCountdown(double seconds)
        {
            if (seconds <= 0.0) return "Flip burn overdue";
            int h = (int)(seconds / 3600.0);
            int m = (int)((seconds % 3600.0) / 60.0);
            int s = (int)(seconds % 60.0);
            return $"Flip T-{h:D2}:{m:D2}:{s:D2}";
        }

        private void OnDestroy()
        {
            if (_coreTex != null) Destroy(_coreTex);
            if (_glowTex != null) Destroy(_glowTex);
        }
    }
}
