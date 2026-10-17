using UnityEditor;
using UnityEngine;

public class RenameBindings : EditorWindow
{
    AnimationClip clip;
    string oldName = "OldBoneName";
    string newName = "NewBoneName";

    [MenuItem("Tools/Rename Animation Bindings")]
    static void Open() => GetWindow<RenameBindings>("Rename Bindings");

    void OnGUI()
    {
        clip = (AnimationClip)EditorGUILayout.ObjectField("Clip", clip, typeof(AnimationClip), false);
        oldName = EditorGUILayout.TextField("Old text", oldName);
        newName = EditorGUILayout.TextField("New text", newName);

        if (GUILayout.Button("Replace in all paths") && clip != null)
        {
            Undo.RecordObject(clip, "Rename bindings");

            foreach (var b in AnimationUtility.GetCurveBindings(clip))
            {
                if (!b.path.Contains(oldName)) continue;
                var curve = AnimationUtility.GetEditorCurve(clip, b);
                AnimationUtility.SetEditorCurve(clip, b, null);
                var nb = b;
                nb.path = b.path.Replace(oldName, newName);
                AnimationUtility.SetEditorCurve(clip, nb, curve);
            }

            foreach (var b in AnimationUtility.GetObjectReferenceCurveBindings(clip))
            {
                if (!b.path.Contains(oldName)) continue;
                var curve = AnimationUtility.GetObjectReferenceCurve(clip, b);
                AnimationUtility.SetObjectReferenceCurve(clip, b, null);
                var nb = b;
                nb.path = b.path.Replace(oldName, newName);
                AnimationUtility.SetObjectReferenceCurve(clip, nb, curve);
            }

            EditorUtility.SetDirty(clip);
            AssetDatabase.SaveAssets();
        }
    }
}