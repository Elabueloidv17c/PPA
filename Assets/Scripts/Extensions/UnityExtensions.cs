using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

public static class UnityExtensions
{
  /// <summary>
  /// Creates a copy of given component inside given gameObject
  /// </summary>
  public static T CopyComponent<T>(this GameObject destination, T original) where T : Component
  {
    System.Type type = original.GetType();
    var dst = destination.GetComponent(type) as T;
    if (!dst) dst = destination.AddComponent(type) as T;
    var fields = type.GetFields();
    foreach (var field in fields)
    {
      if (field.IsStatic) continue;
      field.SetValue(dst, field.GetValue(original));
    }
    var props = type.GetProperties();
    foreach (var prop in props)
    {
      if (!prop.CanWrite || !prop.CanRead || prop.Name == "name") continue;

      try
      {
        prop.SetValue(dst, prop.GetValue(original, null), null);
      }
      catch {}
    }
    return dst as T;
  }

  /// <summary>
  /// Finds Child GameObject with given name
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="name"></param>
  /// <returns></returns>
  public static GameObject GetChildWithName(this GameObject obj, string name)
  {
    Transform trans = obj.transform;
    Transform childTrans = trans.Find(name);
    if (childTrans != null)
    {
      return childTrans.gameObject;
    }
    else
    {
      return null;
    }
  }

  #region GetComponentOnlyInChildren
  /// <summary>
  /// Returns the component of Type type in any of its children excluding the GameObject itself
  /// </summary>
  public static Component GetComponentOnlyInChildren(this GameObject obj, 
                                                     Type type, 
                                                     bool includeInactive = false)
  {
    try
    {
      return obj.GetComponentsInChildren(type, includeInactive)
                .Where(go => go.gameObject != obj).First();
    }
    catch (NullReferenceException)
    {
      #if UNITY_EDITOR
      Debug.LogWarning("Could not find component of type " + type.ToString());
      #endif
      return null;
    }
  }

  /// <summary>
  /// Returns the component of Type T in any of its children excluding the GameObject itself
  /// </summary>
  public static T GetComponentOnlyInChildren<T>(this GameObject obj, 
                                                bool includeInactive = false) 
    where T : Component
  {
    try
    {
      return obj.GetComponentsInChildren<T>(includeInactive)
                .Where(go => go.gameObject != obj).First();
    }
    catch (NullReferenceException)
    {
      #if UNITY_EDITOR
      Debug.LogWarning("Could not find component of type " + typeof(T).ToString());
      #endif
      return null;
    }
  }

  /// <summary>
  /// Returns all components of Type type in any of its children excluding the GameObject itself
  /// </summary>
  public static Component[] GetComponentsOnlyInChildren(this GameObject obj,
                                                        Type type,
                                                        bool includeInactive = false)
  {
    try
    {
      return obj.GetComponentsInChildren(type, includeInactive)
                .Where(go => go.gameObject != obj).ToArray();
    }
    catch (NullReferenceException)
    {
      #if UNITY_EDITOR
      Debug.LogWarning("Could not find component of type " + type.ToString());
      #endif
      return null;
    }
  }

  /// <summary>
  /// Returns all components of Type T in any of its children excluding the GameObject itself
  /// </summary>
  public static T[] GetComponentsOnlyInChildren<T>(this GameObject obj,
                                                   bool includeInactive = false)
    where T : Component
  {
    try
    {
      return obj.GetComponentsInChildren<T>(includeInactive)
                .Where(go => go.gameObject != obj).ToArray();
    }
    catch (NullReferenceException)
    {
      #if UNITY_EDITOR
      Debug.LogWarning("Could not find component of type " + typeof(T).ToString());
      #endif
      return null;
    }
  }

  /// <summary>
  /// Returns the component of Type type in any of its children  excluding the Component itself
  /// </summary>
  public static Component GetComponentOnlyInChildren(this Component obj,
                                                     Type type,
                                                     bool includeInactive = false)
  {
    try
    {
      return obj.GetComponentsInChildren(type, includeInactive)
                .Where(go => go.gameObject != obj.gameObject).First();
    }
    catch (NullReferenceException)
    {
      #if UNITY_EDITOR
      Debug.LogWarning("Could not find component of type " + type.ToString());
      #endif
      return null;
    }
  }

  /// <summary>
  /// Returns the component of Type T in any of its children excluding the Component itself
  /// </summary>
  public static T GetComponentOnlyInChildren<T>(this Component obj,
                                                bool includeInactive = false)
    where T : Component
  {
    try
    {
      return obj.GetComponentsInChildren<T>(includeInactive)
                .Where(go => go.gameObject != obj.gameObject).First();
    }
    catch (NullReferenceException)
    {
      #if UNITY_EDITOR
      Debug.LogWarning("Could not find component of type " + typeof(T).ToString());
      #endif
      return null;
    }
  }

  /// <summary>
  /// Returns all components of Type type in any of its children  excluding the Component itself
  /// </summary>
  public static Component[] GetComponentsOnlyInChildren(this Component obj,
                                                        Type type,
                                                        bool includeInactive = false)
  {
    try
    {
      return obj.GetComponentsInChildren(type, includeInactive)
              .Where(go => go.gameObject != obj.gameObject).ToArray();
    }
    catch (NullReferenceException)
    {
      #if UNITY_EDITOR
      Debug.LogWarning("Could not find component of type " + type.ToString());
      #endif
      return null;
    }
  }

  /// <summary>
  /// Returns all components of Type T in any of its children excluding the Component itself
  /// </summary>
  public static T[] GetComponentsOnlyInChildren<T>(this Component obj,
                                                   bool includeInactive = false)
    where T : Component
  {
    try
    {
      return obj.GetComponentsInChildren<T>(includeInactive)
              .Where(go => go.gameObject != obj.gameObject).ToArray();
    }
    catch (NullReferenceException)
    {
      #if UNITY_EDITOR
      Debug.LogWarning("Could not find component of type " + typeof(T).ToString());
      #endif
      return null;
    }
  }
  #endregion
  
  #if UNITY_EDITOR
  public static IEnumerable<SerializedProperty> GetChildren(this SerializedProperty property)
  {
    property = property.Copy();
    var nextElement = property.Copy();
    bool hasNextElement = nextElement.NextVisible(false);
    if (!hasNextElement)
    {
      nextElement = null;
    }

    property.NextVisible(true);
    while (true)
    {
      if ((SerializedProperty.EqualContents(property, nextElement)))
      {
        yield break;
      }

      yield return property;

      bool hasNext = property.NextVisible(false);
      if (!hasNext)
      {
        break;
      }
    }
  }
  #endif

  public static bool IsPrefab(this GameObject obj)
  {
    return obj.scene.rootCount == 0 || 
           obj.scene.name == obj.transform.root.name;
  }
}
