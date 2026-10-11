/*
 ___________              ______________                ____________ _____________
|            |            |            | |            | |          | |
|            |            |            | |            | |          | |
|            |            |            | |            | |          | |_______
|            |            |____________| |            | |          | |
|            |            |            | |            | |          | |
|__________  |___________ |            | |____________| |__________| |____________

*/
// Generates a tileable 3D cloud noise texture (Texture3D asset):
//   R = Perlin-Worley (base cloud shape)
//   G = Worley at 1x frequency   (large detail)
//   B = Worley at 2x frequency   (medium detail)
//   A = Worley at 4x frequency   (fine detail)
// The texture tiles seamlessly on all three axes.

using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class CloudNoiseGenerator : EditorWindow
{
    static readonly int[] ResolutionValues = { 32, 64, 128 };
    static readonly string[] ResolutionLabels = { "32", "64", "128" };

    int resolution = 128;
    int perlinFrequency = 4;
    int perlinOctaves = 4;
    int worleyFrequency = 4;
    int seed = 1;
    string assetPath = "Assets/CloudNoise3D.asset";

    [MenuItem("Tools/Cloud Noise 3D Generator")]
    static void Open()
    {
        GetWindow<CloudNoiseGenerator>("Cloud Noise 3D");
    }

    void OnGUI()
    {
        EditorGUILayout.HelpBox(
            "R = Perlin-Worley (base shape)\nG, B, A = Worley at 1x, 2x, 4x frequency (detail)\nTileable on all 3 axes.",
            MessageType.Info);

        resolution = EditorGUILayout.IntPopup("Resolution", resolution, ResolutionLabels, ResolutionValues);
        perlinFrequency = EditorGUILayout.IntSlider("Perlin frequency", perlinFrequency, 1, 16);
        perlinOctaves = EditorGUILayout.IntSlider("Perlin octaves", perlinOctaves, 1, 6);
        worleyFrequency = EditorGUILayout.IntSlider("Worley frequency", worleyFrequency, 1, 8);
        seed = EditorGUILayout.IntField("Seed", seed);
        assetPath = EditorGUILayout.TextField("Save to", assetPath);

        if (GUILayout.Button("Generate"))
            Generate();
    }

    void Generate()
    {
        int res = resolution;
        int pf = perlinFrequency;
        int po = perlinOctaves;
        int wf = worleyFrequency;
        int sd = seed;

        var colors = new Color32[res * res * res];

        EditorUtility.DisplayProgressBar("Cloud Noise 3D", "Generating " + res + "^3 voxels...", 0.5f);
        try
        {
            Parallel.For(0, res, z =>
            {
                for (int y = 0; y < res; y++)
                {
                    for (int x = 0; x < res; x++)
                    {
                        float fx = (x + 0.5f) / res;
                        float fy = (y + 0.5f) / res;
                        float fz = (z + 0.5f) / res;

                        float perlin = PerlinFbm(fx, fy, fz, pf, po, sd);
                        float w1 = Worley(fx, fy, fz, wf, sd + 1000);
                        float w2 = Worley(fx, fy, fz, wf * 2, sd + 2000);
                        float w3 = Worley(fx, fy, fz, wf * 4, sd + 3000);

                        // Perlin-Worley: Perlin remapped from [0,1] to [worley,1]
                        float perlinWorley = w1 + perlin * (1f - w1);

                        colors[x + y * res + z * res * res] =
                            new Color32(ToByte(perlinWorley), ToByte(w1), ToByte(w2), ToByte(w3));
                    }
                }
            });

            var tex = new Texture3D(res, res, res, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Repeat;
            tex.filterMode = FilterMode.Bilinear;
            tex.SetPixels32(colors);
            tex.Apply();

            // Overwrite in place if the asset already exists, so material references keep working
            var existing = AssetDatabase.LoadAssetAtPath<Texture3D>(assetPath);
            if (existing != null)
            {
                EditorUtility.CopySerialized(tex, existing);
                EditorUtility.SetDirty(existing);
                DestroyImmediate(tex);
                tex = existing;
            }
            else
            {
                AssetDatabase.CreateAsset(tex, assetPath);
            }

            AssetDatabase.SaveAssets();
            Selection.activeObject = tex;
            EditorGUIUtility.PingObject(tex);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    static byte ToByte(float v)
    {
        return (byte)Mathf.RoundToInt(Mathf.Clamp01(v) * 255f);
    }

    // ---------- Hashing ----------

    static uint Hash(int x, int y, int z, int seed)
    {
        unchecked
        {
            uint h = (uint)seed * 374761393u + (uint)x * 668265263u;
            h = (h ^ (h >> 13)) * 1274126177u;
            h += (uint)y * 2246822519u;
            h = (h ^ (h >> 15)) * 3266489917u;
            h += (uint)z * 2654435761u;
            h = (h ^ (h >> 16)) * 2246822519u;
            return h ^ (h >> 13);
        }
    }

    static float Rand01(uint h)
    {
        return (h & 0xFFFFFFu) / 16777216f;
    }

    static int Wrap(int v, int period)
    {
        int m = v % period;
        return m < 0 ? m + period : m;
    }

    // ---------- Periodic Perlin noise ----------

    static float Fade(float t)
    {
        return t * t * t * (t * (t * 6f - 15f) + 10f);
    }

    static float Grad(uint h, float x, float y, float z)
    {
        switch (h & 15u)
        {
            case 0: return x + y;
            case 1: return -x + y;
            case 2: return x - y;
            case 3: return -x - y;
            case 4: return x + z;
            case 5: return -x + z;
            case 6: return x - z;
            case 7: return -x - z;
            case 8: return y + z;
            case 9: return -y + z;
            case 10: return y - z;
            case 11: return -y - z;
            case 12: return x + y;
            case 13: return -x + y;
            case 14: return -y + z;
            default: return -y - z;
        }
    }

    // x, y, z are in lattice units; the lattice repeats every 'period' cells
    static float Perlin(float x, float y, float z, int period, int seed)
    {
        int xi = (int)System.Math.Floor(x);
        int yi = (int)System.Math.Floor(y);
        int zi = (int)System.Math.Floor(z);

        float xf = x - xi;
        float yf = y - yi;
        float zf = z - zi;

        float u = Fade(xf);
        float v = Fade(yf);
        float w = Fade(zf);

        int x0 = Wrap(xi, period), x1 = Wrap(xi + 1, period);
        int y0 = Wrap(yi, period), y1 = Wrap(yi + 1, period);
        int z0 = Wrap(zi, period), z1 = Wrap(zi + 1, period);

        float n000 = Grad(Hash(x0, y0, z0, seed), xf,      yf,      zf);
        float n100 = Grad(Hash(x1, y0, z0, seed), xf - 1f, yf,      zf);
        float n010 = Grad(Hash(x0, y1, z0, seed), xf,      yf - 1f, zf);
        float n110 = Grad(Hash(x1, y1, z0, seed), xf - 1f, yf - 1f, zf);
        float n001 = Grad(Hash(x0, y0, z1, seed), xf,      yf,      zf - 1f);
        float n101 = Grad(Hash(x1, y0, z1, seed), xf - 1f, yf,      zf - 1f);
        float n011 = Grad(Hash(x0, y1, z1, seed), xf,      yf - 1f, zf - 1f);
        float n111 = Grad(Hash(x1, y1, z1, seed), xf - 1f, yf - 1f, zf - 1f);

        float nx00 = Mathf.Lerp(n000, n100, u);
        float nx10 = Mathf.Lerp(n010, n110, u);
        float nx01 = Mathf.Lerp(n001, n101, u);
        float nx11 = Mathf.Lerp(n011, n111, u);

        float nxy0 = Mathf.Lerp(nx00, nx10, v);
        float nxy1 = Mathf.Lerp(nx01, nx11, v);

        return Mathf.Lerp(nxy0, nxy1, w);
    }

    // x, y, z in [0,1); returns [0,1]
    static float PerlinFbm(float x, float y, float z, int period, int octaves, int seed)
    {
        float sum = 0f, amp = 1f, norm = 0f;
        for (int i = 0; i < octaves; i++)
        {
            sum += amp * Perlin(x * period, y * period, z * period, period, seed + i * 31);
            norm += amp;
            amp *= 0.5f;
            period *= 2;
        }
        return Mathf.Clamp01(0.5f + 0.5f * (sum / norm));
    }

    // ---------- Periodic Worley (cellular) noise ----------

    // x, y, z in [0,1); returns inverted Worley (1 at feature points, fading to 0)
    static float Worley(float x, float y, float z, int period, int seed)
    {
        x *= period;
        y *= period;
        z *= period;

        int ix = (int)System.Math.Floor(x);
        int iy = (int)System.Math.Floor(y);
        int iz = (int)System.Math.Floor(z);

        float best = float.MaxValue;

        for (int dz = -1; dz <= 1; dz++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    int cx = ix + dx;
                    int cy = iy + dy;
                    int cz = iz + dz;

                    // Hash with wrapped cell coordinates so the pattern tiles
                    int wx = Wrap(cx, period);
                    int wy = Wrap(cy, period);
                    int wz = Wrap(cz, period);

                    float px = cx + Rand01(Hash(wx, wy, wz, seed));
                    float py = cy + Rand01(Hash(wx, wy, wz, seed + 101));
                    float pz = cz + Rand01(Hash(wx, wy, wz, seed + 202));

                    float ddx = px - x;
                    float ddy = py - y;
                    float ddz = pz - z;
                    float d = ddx * ddx + ddy * ddy + ddz * ddz;
                    if (d < best) best = d;
                }
            }
        }

        return 1f - Mathf.Clamp01(Mathf.Sqrt(best));
    }
}
