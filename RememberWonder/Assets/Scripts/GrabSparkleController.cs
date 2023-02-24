using Bewildered;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabSparkleController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem sparkleSystem;
    [SerializeField] private BoxCollider triggerColl;
    [Space(5)]
    [SerializeField] private PushPullObject grabbableSparkleOwner;
    [SerializeField] private MeshRenderer sparklingMeshRend;
    [Header("Settings")]
    [SerializeField] private Vector3 triggerSize = Vector3.one;
    [Tooltip("How many particles to emit, for each 6 unity units squared in the surface area of sparklingMesh's axis aligned bounding box. " +
        "I.e., if sparklingMesh is a normal, unrotated cube with scale (1,1,1), sparkleSystem's rate over time will equal this.")]
    [SerializeField] private float sparklesPerUnitCube = 10;
    [SerializeField] private UHashSet<TagString> activatorTags;
    
    private const float UNIT_CUBE_SURFACE_AREA = 6;

    public PushPullObject GrabbableSparkleOwner { get => grabbableSparkleOwner; set => grabbableSparkleOwner = value; }
    public MeshRenderer SparklingMeshRend { get => sparklingMeshRend; set => sparklingMeshRend = value; }

    private void OnValidate() => ValidationUtility.DoOnDelayCall(this, ScaleTrigger);
    private void ScaleTrigger()
    {
        triggerColl.size = triggerSize;
        triggerColl.size = UtilFunctions.InverseScale(triggerColl.size, transform.lossyScale);
    }

    private void Start()
    {
        ScaleTrigger();

        if (!sparklingMeshRend)
        {
            Debug.LogWarning($"{transform.parent.name}'s sparkle system was given a null mesh. Disabling to prevent further warnings.");
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
