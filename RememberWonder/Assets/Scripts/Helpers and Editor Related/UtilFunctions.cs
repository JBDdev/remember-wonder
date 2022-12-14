using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilFunctions
{
    /// <summary>
    /// Gets the renderers of <paramref name="obj"/> and its children, and returns the combined bounds of 
    /// all the active/enabled ones.<br/>
    /// Note this returns <i><b>rendered</b></i> bounds, not the bounds of the object's collider.
    /// </summary>
    /// <remarks>
    /// <b>Developer's Note:</b> Renderer bounds tend to include transparent fringes, i.e. padding. See<br/>
    /// <see cref="UnityEngine.Sprites.DataUtility.GetPadding(Sprite)"/>.
    /// </remarks>
    /// <param name="obj">The object to get the total rendered bounds of.</param>
    /// <returns>A <see cref="Bounds"/> that encapsulates the all the active/enabled renderers of 
    /// <paramref name="obj"/> and its children.</returns>
    public static Bounds GetTotalRenderedBounds(this GameObject obj)
    {
        //If this parent obj isn't active, none of the children will be either, so we can return early.
        if (!obj.activeInHierarchy)
            return new Bounds(obj.transform.position, Vector3.zero);

        Renderer[] rends = obj.GetComponentsInChildren<Renderer>();
        return GetTotalRenderedBoundsNonAlloc(rends, obj.transform.position);
    }
    /// <summary>
    /// Returns the combined <see cref="Bounds"/> of all the active/enabled renderers in <paramref name="rends"/>.
    /// </summary>
    /// <param name="rends">The renderers to get the total rendered <see cref="Bounds"/> of.</param>
    /// <param name="defaultCenter">The default center position to use for zero-sized <see cref="Bounds"/>/failures.</param>
    /// <returns></returns> <inheritdoc cref="GetTotalRenderedBounds(GameObject)"/>
    public static Bounds GetTotalRenderedBoundsNonAlloc(Renderer[] rends, Vector3 defaultCenter = default)
    {
        //If there are no renderers, there's no rendered bounds. Return a zero-sized bounds.
        if (rends == null || rends.Length < 1)
            return new Bounds(defaultCenter, Vector3.zero);

        //Init a bounds with the first enabled/active renderer, then expand it to include all the others that're
        //enabled/active.
        Bounds? totalBounds = null;
        foreach (Renderer rend in rends)
            if (rend.enabled && rend.gameObject.activeInHierarchy)
            {
                if (totalBounds is Bounds tBounds)
                    tBounds.Encapsulate(rend.bounds);
                else
                    totalBounds = rend.bounds;
            }

        //If we didn't get to init a bounds, that means none of the renderers are actually showing. In that case,
        //return a zero-sized bounds.
        return totalBounds is Bounds result
            ? result
            : new Bounds(defaultCenter, Vector3.zero);
    }

    public static Bounds EncapsulateAll(params Bounds[] bounds)
    {
        if (bounds.Length < 1)
            return new Bounds(Vector3.zero, Vector3.zero);

        Bounds result = bounds[0];
        for (int i = 1; i < bounds.Length; i++)
            result.Encapsulate(bounds[i]);

        return result;
    }

    #region Lightly modified from unitycoder via https://gist.github.com/unitycoder/58f4b5d80f423d29e35c814a9556f9d9
    public static void DrawBounds(Bounds b, Color c = default, float duration = 0)
    {
        // bottom, counter-clockwise from back bottom left
        var p1 = new Vector3(b.min.x, b.min.y, b.min.z);    //---
        var p2 = new Vector3(b.max.x, b.min.y, b.min.z);    //+--
        var p3 = new Vector3(b.max.x, b.min.y, b.max.z);    //+-+
        var p4 = new Vector3(b.min.x, b.min.y, b.max.z);    //--+

        Debug.DrawLine(p1, p2, c, duration);
        Debug.DrawLine(p2, p3, c, duration);
        Debug.DrawLine(p3, p4, c, duration);
        Debug.DrawLine(p4, p1, c, duration);

        // top, counter-clockwise from back top left
        var p5 = new Vector3(b.min.x, b.max.y, b.min.z);    //-+-
        var p6 = new Vector3(b.max.x, b.max.y, b.min.z);    //++-
        var p7 = new Vector3(b.max.x, b.max.y, b.max.z);    //+++
        var p8 = new Vector3(b.min.x, b.max.y, b.max.z);    //-++

        Debug.DrawLine(p5, p6, c, duration);
        Debug.DrawLine(p6, p7, c, duration);
        Debug.DrawLine(p7, p8, c, duration);
        Debug.DrawLine(p8, p5, c, duration);

        // sides, counter-clockwise from back left
        Debug.DrawLine(p1, p5, c, duration);
        Debug.DrawLine(p2, p6, c, duration);
        Debug.DrawLine(p3, p7, c, duration);
        Debug.DrawLine(p4, p8, c, duration);
    }

    //Comment included with function:
    // https://forum.unity.com/threads/debug-drawbox-function-is-direly-needed.1038499/
    public static void DrawBox(Vector3 pos, Quaternion rot, Vector3 scale, Color c = default, float duration = 0, bool drawAxes = false)
    {
        Matrix4x4 m = new Matrix4x4();
        m.SetTRS(pos, rot, scale);

        //bottom, counter-clockwise from front bottom left
        var point1 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, 0.5f));  //--+
        var point2 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, 0.5f));   //+-+
        var point3 = m.MultiplyPoint(new Vector3(0.5f, -0.5f, -0.5f));  //+--
        var point4 = m.MultiplyPoint(new Vector3(-0.5f, -0.5f, -0.5f)); //---

        Debug.DrawLine(point1, point2, c, duration);
        Debug.DrawLine(point2, point3, c, duration);
        Debug.DrawLine(point3, point4, c, duration);
        Debug.DrawLine(point4, point1, c, duration);

        //top, counter-clockwise from front top left
        var point5 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, 0.5f));   //-++
        var point6 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, 0.5f));    //+++
        var point7 = m.MultiplyPoint(new Vector3(0.5f, 0.5f, -0.5f));   //++-
        var point8 = m.MultiplyPoint(new Vector3(-0.5f, 0.5f, -0.5f));  //-+-

        Debug.DrawLine(point5, point6, c, duration);
        Debug.DrawLine(point6, point7, c, duration);
        Debug.DrawLine(point7, point8, c, duration);
        Debug.DrawLine(point8, point5, c, duration);

        //corners, counter-clockwise from front left
        Debug.DrawLine(point1, point5, c, duration);
        Debug.DrawLine(point2, point6, c, duration);
        Debug.DrawLine(point3, point7, c, duration);
        Debug.DrawLine(point4, point8, c, duration);

        // optional axis display (original code causes compiler errors; Matrix4x4s don't have definitions for the methods used)
        if (drawAxes)
        {
            Color halfSatC = c - Color.HSVToRGB(0, c.ToHSV().y / 2, 0);
            Debug.DrawRay(pos, rot * (Vector3.forward * scale.x / 2), halfSatC);
            Debug.DrawRay(pos, rot * (Vector3.up * scale.x / 2), halfSatC);
            Debug.DrawRay(pos, rot * (Vector3.right * scale.x / 2), halfSatC);
        }
    }
    public static void DrawBox(Vector3 pos, Quaternion rot, float scale, Color c = default, float duration = 0, bool drawAxes = false)
        => DrawBox(pos, rot, Vector3.one * scale, c, duration, drawAxes);
    #endregion

    /// <summary>
    /// Checks to see if this float is equal to <paramref name="target"/>, within a 
    /// given <paramref name="range"/>.
    /// </summary>
    public static bool EqualWithinRange(this float subject, float target, float range)
        => subject >= target - range && subject <= target + range;

    public static bool EqualWithinRange(this Vector3 subject, Vector3 target, float range)
        => (target - subject).sqrMagnitude <= range * range;

    /// <summary>
    /// Returns a Vector3 where XYZ = HSV, via <see cref="Color.RGBToHSV(Color, out float, out float, out float)"/>.
    /// </summary>
    public static Vector3 ToHSV(this Color c)
    {
        Vector3 result;
        Color.RGBToHSV(c, out result.x, out result.y, out result.z);
        return result;
    }

    /// <summary>
    /// Sets one channel (RGBA, 0123) of a color by <paramref name="val"/>. Optionally allows adding instead.
    /// </summary>
    /// <remarks>Will return the color unchanged if <paramref name="indToAdjust"/> is invalid (i&lt;0, i&gt;3).</remarks>
    public static Color Adjust(this Color c, int indToAdjust, float val, bool addVal = false)
    {
        if (indToAdjust < 0 || indToAdjust > 3) return c;

        c[indToAdjust] = addVal ? c[indToAdjust] + val : val;
        return c;
    }

    public static Vector3 ClampComponents(Vector3 v,
        float xMin, float xMax,
        float yMin, float yMax,
        float zMin, float zMax)
    {
        v.x = Mathf.Clamp(v.x, xMin, xMax);
        v.y = Mathf.Clamp(v.y, yMin, yMax);
        v.z = Mathf.Clamp(v.z, zMin, zMax);
        return v;
    }
    public static Vector3 ClampComponents(Vector3 v, Vector3 minComponents, Vector3 maxComponents) =>
        ClampComponents(v,
            minComponents.x, maxComponents.x,
            minComponents.y, maxComponents.y,
            minComponents.z, maxComponents.z);
    public static Vector3 ClampComponents(Vector3 v, float min, float max) =>
        ClampComponents(v, min, max, min, max, min, max);

    /// <summary>
    /// Returns the closest value to <paramref name="v"/> that's outside the range (<paramref name="rangeMin"/>, 
    /// <paramref name="rangeMax"/>).<br/>
    /// Returns <paramref name="rangeMax"/> if equidistant to both edges.
    /// </summary>
    public static float ClampOutside(float v, float rangeMin, float rangeMax)
    {
        if (v < rangeMin || v > rangeMax)
            return v;

        return v - rangeMin < rangeMax - v
            ? rangeMin
            : rangeMax;
    }

    /// <summary>
    /// Divides two vectors component-wise.
    /// </summary>
    public static Vector3 InverseScale(Vector3 a, Vector3 b) => new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);

    /// <summary>
    /// Scales this transform so that it's sized as if its parent had a scale of (1,1,1).
    /// </summary>
    /// <param name="parentLevel">The number of parents to go up by. 
    /// 0 = parent, 1 = grandparent (parent.parent), etc.</param>
    public static void NegateParentScale(this Transform tform, int parentLevel = 0)
    {
        Transform targetParent = tform.parent;
        for (int i = 0; i < parentLevel; i++)
        {
            if (!targetParent.parent)
                break;

            targetParent = targetParent.parent;
        }

        tform.localScale = InverseScale(tform.localScale, targetParent.localScale);
    }

    /// <summary>
    /// Lerps between <paramref name="from"/>-&gt;<paramref name="mid"/>-&gt;<paramref name="to"/>, based
    /// on <paramref name="t"/>.<br/>
    /// Optionally allows setting the mid point's "time" [0-1] to something other than 0.5.
    /// </summary>
    /// <param name="midTime">
    ///     <paramref name="mid"/>'s "position" along the 0-1 curve between 
    ///     <paramref name="from"/>-&gt;<paramref name="mid"/>-&gt;<paramref name="to"/>.<br/>
    ///     0.5 = equal distance to both end points.
    /// </param>
    public static float Lerp3Point(float from, float mid, float to, float t, float midTime = 0.5f)
    {
        if (t <= midTime)
            return Mathf.Lerp(from, mid, t * 2);
        else
            return Mathf.Lerp(mid, to, (t - 0.5f) * 2);
    }

    /// <summary>
    /// Calls <see cref="GameObject.SetActive(bool)"/> on this game object if it's not null or pending destroy.
    /// </summary>
    /// <returns>Was <see cref="GameObject.SetActive(bool)"/> called successfully?</returns>
    public static bool SafeSetActive(this GameObject obj, bool active)
    {
        if (obj)
        {
            obj.SetActive(active);
            return true;
        }

        return false;
    }

    /// <inheritdoc cref="SafeSetActive(GameObject, bool)"/>
    public static bool SafeSetActive(Component objSource, bool active)
    {
        if (objSource) return objSource.gameObject.SafeSetActive(active);

        return false;
    }

    /// <summary>
    /// Takes a collection and, in the order of the collection, adds all distinct elements 
    /// of it to <paramref name="destination"/>.
    /// </summary>
    /// <param name="clearDest">Whether to <see cref="ICollection{T}.Clear"/> 
    /// <paramref name="destination"/> before adding to it.</param>
    public static void DistinctNonAlloc<T>(IEnumerable<T> source, IList<T> destination, bool clearDest = false)
    {
        if (clearDest)
            destination.Clear();

        foreach (var item in source)
        {
            if (!destination.Contains(item))
            {
                destination.Add(item);
            }
        }
    }

    /// <summary>
    /// Takes an ordered collection and, in the order of the collection, adds its elements to 
    /// <paramref name="destination"/>, sans any elements identical<br/>to the one immediately before.
    /// </summary>
    /// <param name="clearDest">Whether to <see cref="ICollection{T}.Clear"/> 
    /// <paramref name="destination"/> before adding to it.</param>
    public static void RemoveAdjacentDuplicatesNonAlloc<T>(
        IList<T> source, IList<T> destination, bool clearDest = true)
    {
        if (clearDest)
            destination.Clear();

        for (int i = 0; i < source.Count; i++)
        {
            if (i < 1 || !EquCompEquals(source[i - 1], source[i]))
            {
                destination.Add(source[i]);
            }
        }
    }

    /// <summary>
    /// A shorthand function for <see cref="EqualityComparer{T}.Default.Equals(T, T)"/>.
    /// </summary>
    public static bool EquCompEquals<T>(T a, T b) => EqualityComparer<T>.Default.Equals(a, b);

    public static bool CompareTagInParentsAndChildren(Transform subject, string tag,
        int levelsUp = int.MaxValue, int levelsDown = int.MaxValue,
        bool checkSelf = true, bool parentsFirst = true)
    {
        if (parentsFirst)
        {
            if (CompareTagInParents(subject, tag, levelsUp, checkSelf)) return true;
            if (CompareTagInChildren(subject, tag, levelsDown, false)) return true;
        }
        else
        {
            if (CompareTagInChildren(subject, tag, levelsDown, checkSelf)) return true;
            if (CompareTagInParents(subject, tag, levelsUp, false)) return true;
        }

        return false;
    }

    public static bool CompareTagInParents(Transform subject, string tag, int levels = int.MaxValue, bool checkSelf = true)
    {
        if (checkSelf && subject.CompareTag(tag)) return true;

        //Get the parent of subject. Then, so long as there are parents (and we haven't gone past the number of
        //levels specified), CompareTag on those parents.
        var next = subject.parent;
        while (next && levels > 0)
        {
            if (next.CompareTag(tag)) return true;

            next = next.parent;
            levels--;
        }

        return false;
    }
    public static bool CompareTagInParents(Component subject, string tag, int levels = int.MaxValue, bool checkSelf = true)
        => CompareTagInParents(subject.transform, tag, levels, checkSelf);

    public static bool CompareTagInChildren(Transform subject, string tag, int levels = int.MaxValue, bool checkSelf = true)
    {
        if (checkSelf && subject.CompareTag(tag)) return true;
        if (levels < 1) return false;

        //Compare the tags on each child. If this is the last level (level <= 1), the recursion won't continue inside these checks.
        foreach (Transform child in subject)
            if (CompareTagInChildren(child, tag, levels - 1, true)) return true;

        return false;
    }
    public static bool CompareTagInChildren(Component subject, string tag, int levels = int.MaxValue, bool checkSelf = true)
        => CompareTagInChildren(subject.transform, tag, levels, checkSelf);
}