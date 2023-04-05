using Bewildered;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabSparkleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem sparkleSystem;
    [SerializeField] private BoxCollider triggerCollider;
    [Space(5)]
    [SerializeField] private PushPullObject grabbableSparkleOwner;
    [SerializeField] private MeshRenderer sparklingMeshRend;
    [Header("Settings")]
    [Tooltip("How many particles to emit, for each 6 unity units squared in the surface area of sparklingMesh's axis aligned bounding box. " +
        "I.e., if sparklingMesh is a normal, unrotated cube with scale (1,1,1), sparkleSystem's rate over time will equal this.")]
    [SerializeField] private float sparklesPerUnitCube = 10;
    [Tooltip("Trigger Collider is sized to fit the Sparkling Mesh Rend. How much extra size (in world space) should this trigger have?")]
    [SerializeField] private Vector3 triggerSizeOffset = Vector3.zero;
    [SerializeField] private Vector3 triggerPosOffset = Vector3.zero;
    [SerializeField] private UHashSet<TagString> activatorTags;

    private const float UNIT_CUBE_SURFACE_AREA = 6;

#if UNITY_EDITOR
    private void OnValidate() => ValidationUtility.DoOnDelayCall(this, () => InitTrigger());
    public void InitTrigger(Vector3? scaleOffsetOverride = null, Vector3? posOffsetOverride = null)
    {
        if (!triggerCollider) return;

        var serializedSelf = new UnityEditor.SerializedObject(this);
        var serializedTrigger = new UnityEditor.SerializedObject(serializedSelf.FindProperty("triggerCollider").objectReferenceValue);

        Vector3 newSize = Vector3.one;
        Vector3 newCenter = Vector3.zero;

        if (sparklingMeshRend)
        {
            newSize = UtilFunctions.InverseScale(sparklingMeshRend.bounds.size, transform.lossyScale);
            newCenter = transform.InverseTransformPoint(sparklingMeshRend.bounds.center);
        }

        if (scaleOffsetOverride is Vector3 newScaleOffset)
            serializedSelf.FindProperty("triggerSizeOffset").vector3Value = newScaleOffset;
        if (posOffsetOverride is Vector3 newPosOffset)
            serializedSelf.FindProperty("triggerPosOffset").vector3Value = newPosOffset;

        newSize += UtilFunctions.InverseScale(triggerSizeOffset, transform.lossyScale);
        newCenter += UtilFunctions.InverseScale(triggerPosOffset, transform.lossyScale);

        //Go through all axes of trigger size,
        for (int i = 0; i < 3; i++)
        {
            //and make all zero/negative values a very small positive number instead.
            //  (BoxColliders don't support negative size.)
            newSize[i] = newSize[i] <= 0 ? 1E-5f : newSize[i];
        }

        serializedTrigger.FindProperty("m_Size").vector3Value = newSize;
        serializedTrigger.FindProperty("m_Center").vector3Value = newCenter;

        serializedTrigger.ApplyModifiedProperties();
        serializedSelf.ApplyModifiedProperties();
    }
#endif

    private void Start()
    {
        if (!sparklingMeshRend)
        {
            var salientName = transform.parent.parent ? transform.parent.parent.name : transform.parent.name;
            Debug.LogWarning($"Grab sparkle system on \"{salientName}\" was given a null mesh. Disabling to prevent further warnings.");
            gameObject.SetActive(false);
            return;
        }

        var sparkleSysShape = sparkleSystem.shape;
        sparkleSysShape.meshRenderer = sparklingMeshRend;
        //The normal offset of mesh renderer particles is scaled by the hierarchical scale of the renderer's object.
        //Negate that by dividing the offset by the largest axis of scale.
        //  If scale's non-uniform, this cuts the smaller axes more than desired; this is unfortunate but necessary
        sparkleSysShape.normalOffset /= UtilFunctions.MaxAbs(
            sparklingMeshRend.transform.lossyScale.x,
            sparklingMeshRend.transform.lossyScale.y,
            sparklingMeshRend.transform.lossyScale.z);

        var sparkleSysEmission = sparkleSystem.emission;
        sparkleSysEmission.rateOverTime = sparklesPerUnitCube * (sparklingMeshRend.bounds.GetSurfaceArea() / UNIT_CUBE_SURFACE_AREA);

        var sparkleSysMain = sparkleSystem.main;
        sparkleSysMain.customSimulationSpace = sparklingMeshRend.transform.parent;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!activatorTags.Contains(other.tag)) return;

        if (!sparkleSystem.isPlaying && (!grabbableSparkleOwner || !grabbableSparkleOwner.IsGrabbed))
        {
            sparkleSystem.Play();
            sparkleSystem.Emit(Mathf.RoundToInt(sparkleSystem.emission.rateOverTime.constant) * 2);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (!activatorTags.Contains(other.tag) || !sparkleSystem.isPlaying) return;

        sparkleSystem.Stop();
    }
    private void Update()
    {
        if (sparkleSystem.isPlaying && grabbableSparkleOwner && grabbableSparkleOwner.IsGrabbed)
        {
            sparkleSystem.Stop();
        }
    }
}
