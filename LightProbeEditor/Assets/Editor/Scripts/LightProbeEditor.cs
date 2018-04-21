using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using System.Collections.Generic;


public class LightProbeEditor : EditorWindow
{

    #region Filed

    float scaleNumber = 1.0f;

    float realScaleNumber = 1.0f;

    static SphericalHarmonicsL2[] probes;
    static List<SphericalHarmonicsL2> tempProbes;

    Color ambient;
    Color realAmbient;

    #endregion

    #region Methods

    [MenuItem("Window/LightProbeEditor")]
    static void InitWindow()
    {
        LightProbeEditor window = (LightProbeEditor)EditorWindow.GetWindow(typeof(LightProbeEditor));


        if (!ExtraLightProbeData())
        {
            return;
        }
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("LightProbe AmbientColor and Intensity Settings", EditorStyles.boldLabel);

        scaleNumber = EditorGUILayout.Slider("LightProbe Intensity Scale", scaleNumber, 0, 9);
        ambient = EditorGUILayout.ColorField("Ambient Color", ambient);


        if (GUILayout.Button("Reset"))
        {
            this.scaleNumber = 1;
            ScaleLightProbeData(this.scaleNumber);
        }

        if (GUILayout.Button("Save Ambient"))
        {
            SetLightProbeAmbientColor(this.realAmbient, true);
        }

        if (GUILayout.Button("Create ProbeLight Asset"))
        {
            AssetDatabase.CreateAsset(Instantiate(LightmapSettings.lightProbes), "Assets/lightProbe.asset");
        }

        if (Mathf.Abs(this.scaleNumber - this.realScaleNumber) > 0.01f)
        {
            this.realScaleNumber = this.scaleNumber;

            ScaleLightProbeData(realScaleNumber);
        }

        if(this.ambient != this.realAmbient)
        {
            this.realAmbient = this.ambient;
            SetLightProbeAmbientColor(this.realAmbient);
        }
    }

    static bool ExtraLightProbeData()
    {
        probes = LightmapSettings.lightProbes.bakedProbes;

        tempProbes = new List<SphericalHarmonicsL2>(probes.Length);

        if (probes == null)
        {
            Debug.LogError("there is no probe data!!!!");
            return false;
        }

        return true;
    }

    static void ScaleLightProbeData(float scale)
    {
        tempProbes.Clear();
        for (int i = 0; i < probes.Length; i++)
        {
            var probe = probes[i];

            tempProbes.Add(probe * scale);
        }

        ReplaceLightProbe(tempProbes.ToArray());
    }

    static void SetLightProbeAmbientColor(Color color, bool isSave = false)
    {
        if(tempProbes == null){
            tempProbes = new List<SphericalHarmonicsL2>();
        }
        tempProbes.Clear();
        for (int i = 0; i < probes.Length; i++)
        {
            var probe = probes[i];

            probe.AddAmbientLight(color);
            tempProbes.Add(probe);

            if(isSave)
            {
                probes[i] = probe;
            }
        }

        ReplaceLightProbe(tempProbes.ToArray());

    }

    public static void ReplaceLightProbe(SphericalHarmonicsL2[] probeData)
    {
        var lightProbes = LightmapSettings.lightProbes;
        lightProbes.bakedProbes = probeData;
        LightmapSettings.lightProbes = lightProbes;
        EditorUtility.SetDirty(lightProbes);
        SceneView.RepaintAll();
    }

#endregion
}
