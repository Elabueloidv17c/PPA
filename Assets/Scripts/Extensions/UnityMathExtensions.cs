using UnityEngine;

public static class UnityMathExtensions
{
  public static float DistanceSqrd(this Vector2 v, Vector2 other)
  {
    return (v - other).sqrMagnitude;
  }

  /// <summary>
  /// Project vector a onto b
  /// </summary>
  public static float Project(this Vector2 a, Vector2 b)
  {
    return Vector2.Dot(a, b.normalized);
  }

  /// <summary>
  /// Get Vector from projecting vector a onto b
  /// </summary>
  public static Vector2 VProject(this Vector2 a, Vector2 b)
  {
    Vector2 nb = b.normalized;
    float dp = Vector2.Dot(a, nb);
    return new Vector2(dp * nb.x, dp * nb.y);
  }

  /// <summary>
  /// Component-wise multiplication
  /// </summary>
  public static Vector2 CompMultiply(this Vector2 a, Vector2 b)
  {
    return new Vector2(a.x * b.x, a.y * b.y);
  }

  public static float DistanceSqrd(this Vector3 v, Vector3 other)
  {
    return (v - other).sqrMagnitude;
  }

  /// <summary>
  /// Project vector a onto b
  /// </summary>
  public static float Project(this Vector3 a, Vector3 b)
  {
    return Vector3.Dot(a, b.normalized);
  }

  /// <summary>
  /// Get Vector from projecting vector a onto b
  /// </summary>
  public static Vector3 VProject(this Vector3 a, Vector3 b)
  {
    Vector3 nb = b.normalized;
    float dp = Vector3.Dot(a, nb);
    return new Vector3(dp * nb.x, dp * nb.y);
  }

  /// <summary>
  /// Component-wise multiplication
  /// </summary>
  public static Vector3 CompMultiply(this Vector3 a, Vector3 b)
  {
    return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
  }

  /// <summary>
  /// Component-wise Max function
  /// </summary>
  public static Vector2 MaxComp(this Vector2 a, Vector2 b)
  {
    return new Vector2(Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
  }

  /// <summary>
  /// Component-wise Min function
  /// </summary>
  public static Vector2 MinComp(this Vector2 a, Vector2 b)
  {
    return new Vector2(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y));
  }
}
