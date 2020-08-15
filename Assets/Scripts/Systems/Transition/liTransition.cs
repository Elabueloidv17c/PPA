using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Requires a Canvas Group for this to work.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class liTransition : MonoBehaviour
{
  /// <summary>
  /// The Reference to the Transition Effect.
  /// </summary>
  [SerializeField]
  private Animation transitionAnimation;

  public CanvasGroup m_elementsToTransition;

  public bool isFadeDone { get; set; }

  private void Awake()
  {
    m_elementsToTransition = GetComponent<CanvasGroup>();
    isFadeDone = true;
  }

  // Update is called once per frame
  void Update()
  {
    //m_alphaSum += Time.deltaTime;
    //m_elementsToTransition.alpha = m_alphaSum % 1.0f;

    //if (Input.GetKeyDown(KeyCode.P))
    //{
    //  FadeIn();
    //}

    //if (Input.GetKeyDown(KeyCode.O))
    //{
    //  FadeOut();
    //}
  }
  public IEnumerator FadeIn()
  {
    isFadeDone = false;
    foreach (var value in reappear())
    {
      yield return value;
    }
    isFadeDone = true;
    yield return true;
  }

  public IEnumerator FadeOut()
  {
    isFadeDone = false;
    foreach (var value in fade())
    {
      yield return value;
    }
    isFadeDone = true;
    yield return 1;
  }

  public IEnumerable fade()
  {
    float totalTransition = 1.0f;
    while (totalTransition > 0.000000f)
    {
      totalTransition -= Time.deltaTime;
      m_elementsToTransition.alpha = totalTransition;
      yield return false;
    }

    yield return true;
  }

  /// <summary>
  /// 
  /// </summary>
  /// <returns></returns>
  public IEnumerable reappear()
  {
    float totalTransition = 0.0f;
    while (totalTransition < 1.000000f)
    {
      totalTransition += Time.deltaTime;
      m_elementsToTransition.alpha = totalTransition;
      yield return false;
    }

    yield return true;
  }
}
