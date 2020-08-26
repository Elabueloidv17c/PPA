using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class used for transitioning effects for the UI of the game.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class liTransition : MonoBehaviour
{
  /// <summary>
  /// The Reference to the Transition animation.
  /// </summary>
  [SerializeField]
  private Animation transitionAnimation;

  /// <summary>
  /// Is the elements the class controls.
  /// </summary>
  public CanvasGroup elementsInCanvasGroup;

  public bool isFadeDone { get; set; }

  /// <summary>
  /// How much time does it take to for the transition to happen.
  /// </summary>
  public float transitionTime = 1.0f;

  /// <summary>
  /// 
  /// </summary>
  protected float transitionSpeed; 
  private void Awake()
  {
    elementsInCanvasGroup = GetComponent<CanvasGroup>();
    isFadeDone = true;
  }

  /// <summary>
  /// Makes the elements in the CanvasGroup fade into existence (make them visible) .
  /// </summary>
  /// <remarks> Is Meant to be used with a StartCoroutine(FadeIn()) call </remarks>
  /// <returns></returns>
  public IEnumerator FadeIn()
  {
    
    isFadeDone = false;
    transitionSpeed = 1.0f / Mathf.Max(transitionTime, float.Epsilon);
    foreach (var value in reappear())
    {
      yield return value;
    }
    isFadeDone = true;
    yield return true;
  }

  /// <summary>
  /// Makes the elements in the CanvasGroup fade out of existence (make them invisible).
  /// </summary>
  /// <returns></returns>
  public IEnumerator FadeOut()
  {
    isFadeDone = false;
    transitionSpeed = 1.0f / Mathf.Max(transitionTime, float.Epsilon);
    foreach (var value in vanishEffect())
    {
      yield return value;
    }
    isFadeDone = true;
    yield return true;
  }

  /// <summary>
  /// Makes the elements attached to the Canvas Group disappear.
  /// </summary>
  /// <returns> "true" when the effect is complete, otherwise it returns "false"</returns>
  private IEnumerable vanishEffect()
  {
    float totalTransition = 1.0f;
    while (totalTransition > 0.000000f)
    {
      totalTransition -= (Time.deltaTime * transitionSpeed);
      elementsInCanvasGroup.alpha = totalTransition;
      yield return false;
    }

    yield return true;
  }

  /// <summary>
  /// Makes the elements attached to the Canvas Group reappear.
  /// </summary>
  /// <returns> "true" when the effect is complete, otherwise it returns "false"</returns>
  private IEnumerable reappear()
  {
    float totalTransition = 0.0f;
    while (totalTransition < 1.000000f)
    {
      totalTransition += (Time.deltaTime * transitionSpeed);
      elementsInCanvasGroup.alpha = totalTransition;
      yield return false;
    }

    yield return true;
  }
}
