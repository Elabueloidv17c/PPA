using UnityEngine;
using UnityEngine.EventSystems;

public class liTopPanelAutoHide 
    : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    RectTransform trfm;
    bool pointerOver;
    float originPos;

    void Start()
    {
        trfm = GetComponent<RectTransform>();
        originPos = trfm.position.y;
    }
    

    public void OnPointerEnter(PointerEventData eventData)
    {
        pointerOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pointerOver = false;
    }

    // Update is called once per frame
    void Update()
    {
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
