using UnityEditor;
using UnityEngine;

/// <summary>
/// Turns the collision proxies modelled in Blender (objects named "col_*", parented to the deform bones)
/// into invisible MeshColliders when a model is imported. They follow the animation because they are
/// children of the bones. The model root gets a kinematic Rigidbody so PhysX treats them as moving colliders.
/// </summary>
public class GodzillaColliderPostprocessor : AssetPostprocessor
{
    private const string ProxyPrefix = "col_";
    private const string ProxyLayer = "whatIsGround";

    private void OnPostprocessModel(GameObject root)
    {
        bool hasProxies = false;
        int layer = LayerMask.NameToLayer(ProxyLayer);

        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            if (!t.name.StartsWith(ProxyPrefix))
                continue;

            hasProxies = true;
            MeshFilter filter = t.GetComponent<MeshFilter>();
            Mesh mesh = filter != null ? filter.sharedMesh : null;

            // Back proxies are open surfaces cut from the body, so they can't be convex.
            // Non-convex is allowed because the root Rigidbody is kinematic.
            MeshCollider meshCollider = t.GetComponent<MeshCollider>();
            if (meshCollider == null)
                meshCollider = t.gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            meshCollider.convex = false;

            Renderer renderer = t.GetComponent<Renderer>();
            if (renderer != null)
                Object.DestroyImmediate(renderer);
            if (filter != null)
                Object.DestroyImmediate(filter);

            if (layer >= 0)
                t.gameObject.layer = layer;
        }

        if (!hasProxies)
            return;

        Rigidbody body = root.GetComponent<Rigidbody>();
        if (body == null)
            body = root.AddComponent<Rigidbody>();
        body.isKinematic = true;
        body.useGravity = false;
    }
}
