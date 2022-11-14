using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InputHub
{
    private static BaseControls instance;
    /// <summary>
    /// The hub's input instance, from which everything else is accessed.<br/>
    /// If the instance is null, it will be instantiated, enabled, and then returned.
    /// </summary>
    public static BaseControls Inst
    {
        get
        {
            if (instance == null)
            {
                instance = new BaseControls();
                instance.Enable();
            }

            return instance;
        }
    }

    public static void ToggleInputEnabled(bool enabled)
    {
        if (enabled)
            Inst.Enable();
        else
            Inst.Disable();
    }

    /// <summary>
    /// This part's inclusion is inspired by this forum post;
    /// <see href="https://forum.unity.com/goto/post?id=6982748#post-6982748"/><br/>
    /// It resets the base controls instance to null on load, specifically during subsystem registration.<br/>
    /// I don't know if this is necessary. I reckon it can't hurt.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetInstance()
    {
        instance = null;
    }
}