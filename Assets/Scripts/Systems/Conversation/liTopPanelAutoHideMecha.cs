using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Auto hides the top panel in the dialog window.
/// Attach to said top panel.
/// </summary>
public class liTopPanelAutoHideMecha 
    : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// Top panel's UI transform.
    /// </summary>
    RectTransform trfm;

    /// <summary>
    /// Signals if pointer is over top panel.
    /// </summary>
    bool pointerOver;

    /// <summary>
    /// Original Y position of the top panel.
    /// </summary>
    float originPos;

    void Start()
    {
        // Initialize top panel auto hide mechanism
        trfm = GetComponent<RectTransform>();
        originPos = trfm.position.y;
    }
    
    /// <summary>
    /// Invoked by unity's event system when pointer enters top panel area.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerOver = true;
    }

    /// <summary>
    /// Invoked by unity's event system when pointer exits top panel area.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        pointerOver = false;
    }

    void Update()
    {
        // When pointer is over move panel towards it's shown position
        // Otherwise move panel towards it's hidden position
        if(pointerOver) {
            trfm.position = Vector2.MoveTowards(
                trfm.position, 
                new Vector2(trfm.position.x, originPos - 110), 
                5
            );
        }
        else {
            trfm.position = Vector2.MoveTowards(
                trfm.position, 
                new Vector2(trfm.position.x, originPos), 
                5
            );
        }
    }
}
