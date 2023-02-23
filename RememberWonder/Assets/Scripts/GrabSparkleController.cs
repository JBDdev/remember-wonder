using Bewildered;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabSparkleController : MonoBehaviour
{
    [SerializeField] private ParticleSystem sparkleSystem;
    [SerializeField] private PushPullObject sparklingGrabbable;
    [SerializeField] private MeshRenderer sparklingMeshObj;
    [Tooltip("How many particles to emit, for each 6 unity units squared in the surface area of sparklingMesh's axis aligned bounding box. " +
        "I.e., if sparklingMesh is a normal, unrotated cube with scale (1,1,1), sparkleSystem's rate over time will equal this.")]
    [SerializeField] private float sparklesPerUnitCube = 10;
    [SerializeField] private UHashSet<TagString> activatorTags;

    const float UNIT_CUBE_SURFACE_AREA = 6;

    private void Start()
    {
        if (!sparklingMeshObj)
        {
            Debug.LogWarning($"{transform.parent.name}'s sparkle system was given a null mesh. Disabling to prevent further warnings.");
            gameObject.SetActive(false);
            return;
        }

        var sparkleSysShape = sparkleSystem.shape;
        sparkleSysShape.meshRenderer = sparklingMeshObj;

        var sparkleSysEmission = sparkleSystem.emission;
        sparkleSysEmission.rateOverTime = sparklesPerUnitCube * (sparklingMeshObj.bounds.GetSurfaceArea() / UNIT_CUBE_SURFACE_AREA);

        var sparkleSysMain = sparkleSystem.main;
        sparkleSysMain.customSimulationSpace = sparklingMeshObj.transform.parent;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!activatorTags.Contains(other.tag)) return;

        if (!sparkleSystem.isPlaying && (!sparklingGrabbable || !sparklingGrabbable.IsGrabbed))
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
        if (sparkleSystem.isPlaying && sparklingGrabbable && sparklingGrabbable.IsGrabbed)
        {
            sparkleSystem.Stop();
        }
    }
}
