using UnityEngine;
using UnityEngine.UI;

public class liItemSlot : MonoBehaviour
{
    [HideInInspector]
    public Image image;

    [HideInInspector]
    public Button button;
    
    [HideInInspector]
    public int itemID = -1;
    
    [HideInInspector]
    public int itemInstIndex = -1;

    void Start()
    {
        image = transform.GetChild(0).GetComponent<Image>();
        button = GetComponent<Button>();
    }
}
