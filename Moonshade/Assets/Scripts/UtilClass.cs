using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public static class UtilClass
{
    private static Camera _camera;

    public static Vector3 GetMouseWorldPosition(Camera cam = default)
    {
        if (_camera == null && cam == null)
        {
            cam = Camera.main;
            _camera = cam;
        }
        else if (cam != null)
        {
            _camera = cam;
        }

        if (SystemInfo.deviceType != DeviceType.Handheld)
        {
            return _camera.ScreenToWorldPoint(Input.mousePosition);
        }
        else
        {
            return _camera.ScreenToWorldPoint(Input.GetTouch(Input.touchCount - 1).position);
        }
    }

    public static bool IsPressing()
    {
        if (SystemInfo.deviceType != DeviceType.Handheld)
        {
            return Input.GetMouseButton(0);
        }
        else
        {
            if (Input.touchCount == 0)
                return false;
            return true;
        }
    }

    public static Vector3 GetMousePosition()
    {
        if (SystemInfo.deviceType != DeviceType.Handheld)
        {
            return Input.mousePosition;
        }
        else
        {
            if (Input.touchCount == 0)
                return Vector3.zero;
            return Input.GetTouch(Input.touchCount - 1).position;
        }
    }

    public static Vector3 BuildVector3FromSameFloat(float val)
    {
        return new Vector3(val, val, val);
    }

    public static float GetAspectRatio()
    {
        return (float)Screen.width / Screen.height;
    }

    public static string GetSystemLanguage()
    {
        return CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
    }

    /// <summary>
    /// Finds and returns closest target nearby
    /// </summary>
    /// <param name="radius"> target check radius </param>
    /// <returns>Target or null</returns>
    public static T GetTargetNearby<T>(Vector3 from, float radius = 9999f, T[] excludeThese = null) where T : Component
    {
        Collider2D[] targetCols = Physics2D.OverlapCircleAll(from, radius);
        float minDistance = 999f;
        Collider2D closestCollider = null;
        foreach (Collider2D col in targetCols)
        {
            if (col.TryGetComponent(out T t))
            {
                if (excludeThese != null && excludeThese.Contains(t))
                    continue;

                float dist = Vector2.Distance(from, col.transform.position);
                if (dist < minDistance)
                {
                    closestCollider = col;
                    minDistance = dist;
                }
            }
        }

        if (closestCollider == null)
        {
            return null;
        }

        T _target = closestCollider.GetComponent<T>();
        return _target;
    }

    /// <summary>
    /// Finds and returns closest target nearby
    /// </summary>
    /// <param name="radius"> target check radius </param>
    /// <returns>Target or null</returns>
    public static Transform GetTargetTransformNearby<T>(Vector3 from, float radius = 9999f, T[] excludeThese = null)
    {
        Collider2D[] targetCols = Physics2D.OverlapCircleAll(from, radius);
        float minDistance = 999f;
        Collider2D closestCollider = null;
        foreach (Collider2D col in targetCols)
        {
            if (col.TryGetComponent(out T t))
            {
                if (excludeThese != null && excludeThese.Contains(t))
                    continue;

                float dist = Vector2.Distance(from, col.transform.position);
                if (dist < minDistance)
                {
                    closestCollider = col;
                    minDistance = dist;
                }
            }
        }

        if (closestCollider == null)
        {
            return null;
        }

        return closestCollider.transform;
    }

    /// <summary>
    /// Finds and returns random target nearby
    /// </summary>
    /// <param name="radius"> target check radius </param>
    /// <returns>Target or null</returns>
    public static T GetRandomTargetInArea<T>(Vector3 from, float radius = 9999f) where T : Component
    {
        Collider2D[] targetCols = Physics2D.OverlapCircleAll(from, radius);
        List<T> targets = new();
        foreach (Collider2D col in targetCols)
        {
            if (col.TryGetComponent(out T t))
            {
                targets.Add(t);
            }
        }

        if (targets.Count == 0)
        {
            return null;
        }

        return targets[Random.Range(0, targets.Count)];
    }

    public static Vector3 GetRandomPointInArea(Vector3 from, float minRadius = 0f, float maxRadius = 100f)
    {
        float xVal = from.x + GetRandomValueInRange(minRadius, maxRadius);
        float yVal = 0;
        float zVal = from.z + GetRandomValueInRange(minRadius, maxRadius);

        return new Vector3(xVal, yVal, zVal);
    }

    private static float GetRandomValueInRange(float minRadius, float maxRadius)
    {
        float value;
        do
        {
            value = Random.Range(-maxRadius, maxRadius);
        } while (Mathf.Abs(value) < minRadius);

        return value;
    }

    /// <summary>
    /// Finds and returns all targets in area
    /// </summary>
    /// <param name="radius"> targets check radius </param>
    /// <returns>Targets</returns>
    public static List<T> GetAllTargetsInArea<T>(Vector3 from, float radius = 9999f, T[] excludeThese = null)
        where T : Component
    {
        Collider2D[] targetCols = Physics2D.OverlapCircleAll(from, radius);
        var res = new List<T>();
        foreach (Collider2D col in targetCols)
        {
            if (col.TryGetComponent(out T t))
            {
                if (excludeThese != null && excludeThese.Contains(t))
                    continue;

                res.Add(t);
            }
        }

        return res;
    }

    /// <summary>
    /// Finds and returns closest target nearby with excluded type option
    /// </summary>
    /// <param name="radius"> target check radius </param>
    /// <returns>Target or null</returns>
    public static T GetTargetNearbyWithExcluded<T>(Vector3 from, float radius = 9999f, Type excludedType = default)
        where T : Component
    {
        Collider2D[] targetCols = Physics2D.OverlapCircleAll(from, radius);
        float minDistance = 999f;
        Collider2D closestCollider = null;
        foreach (Collider2D col in targetCols)
        {
            if (col.TryGetComponent(out T t))
            {
                if (excludedType != default && excludedType == typeof(T))
                    continue;

                float dist = Vector2.Distance(from, col.transform.position);
                if (dist < minDistance)
                {
                    closestCollider = col;
                    minDistance = dist;
                }
            }
        }

        if (closestCollider == null)
        {
            return null;
        }

        T _target = closestCollider.GetComponent<T>();
        return _target;
    }
}