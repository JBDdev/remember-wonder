using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SourceSettings
{
    public bool loop;
    [Range(0, 256)] public int priority = 128;
    [Range(0, 1)] public float volume = 1;
    [Range(-3, 3)] public float pitch = 1;
    [Range(-1, 1)] public float stereoPan = 0;
    [Range(0, 1)] public float spatialBlend = 0;
    [Range(0, 1.1f)] public float reverbZoneMix = 0;
    [Range(0, 5)] public float dopplerLevel = 0;
    [Range(0, 360)] public float spread = 0;
    public AudioRolloffMode volumeRolloff = AudioRolloffMode.Logarithmic;
    [Min(0)] public float minDistance = 1;
    [Min(0.01f)] public float maxDistance = 500;

    public bool useLoop;
    public bool usePriority;
    public bool useVolume;
    public bool usePitch;
    public bool useStereoPan;
    public bool useSpatialBlend;
    public bool useReverbZoneMix;
    public bool useDopplerLevel;
    public bool useSpread;
    public bool useVolumeRolloff;
    public bool useMinDistance;
    public bool useMaxDistance;

    public void ApplyToSource(ref AudioSource source)
    {
        if (useLoop) source.loop = loop;
        if (usePriority) source.priority = priority;
        if (useVolume) source.volume = volume;
        if (usePitch) source.pitch = pitch;
        if (useStereoPan) source.panStereo = stereoPan;
        if (useSpatialBlend) source.spatialBlend = spatialBlend;
        if (useReverbZoneMix) source.reverbZoneMix = reverbZoneMix;
        if (useDopplerLevel) source.dopplerLevel = dopplerLevel;
        if (useSpread) source.spread = spread;
        if (useVolumeRolloff) source.rolloffMode = volumeRolloff;
        if (useMinDistance) source.minDistance = minDistance;
        if (useMaxDistance) source.maxDistance = maxDistance;
    }

    public void InheritFromSource(AudioSource source, bool turnOnEverything)
    {
        loop = source.loop;
        priority = source.priority;
        volume = source.volume;
        pitch = source.pitch;
        stereoPan = source.panStereo;
        spatialBlend = source.spatialBlend;
        reverbZoneMix = source.reverbZoneMix;
        dopplerLevel = source.dopplerLevel;
        spread = source.spread;
        volumeRolloff = source.rolloffMode;
        minDistance = source.minDistance;
        maxDistance = source.maxDistance;

        if (turnOnEverything)
        {
            useLoop = usePriority = useVolume = usePitch = useStereoPan
                = useSpatialBlend = useReverbZoneMix = useDopplerLevel
                = useSpread = useVolumeRolloff = useMinDistance = useMaxDistance = true;
        }
    }
}