using UnityEngine;
using UnityEngine.UI;

using Text = TMPro.TextMeshProUGUI;

public class liItemSlot : MonoBehaviour
{
    [HideInInspector]
    public Image image;

    [HideInInspector]
    public Button button;

    [HideInInspector]
    public Text text;
    
    [HideInInspector]
    public int itemID = -1;
    
    [HideInInspector]
    public int itemInstIndex = -1;

    void Awake()
    {
        image = transform.GetChild(0).GetComponent<Image>();
        button = GetComponent<Button>();
        text = transform.GetChild(1).GetComponent<Text>();
    }
}
