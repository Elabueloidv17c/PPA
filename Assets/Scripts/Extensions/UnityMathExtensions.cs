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
}
