using TMPro;
using UnityEngine;

public class TextMeshProController : MonoBehaviour
{
    public TextMeshProUGUI myTextMeshProUGUI;

    void Start()
    {
        myTextMeshProUGUI = GetComponent<TextMeshProUGUI>();

        if (myTextMeshProUGUI == null)
        {
            Debug.LogError("TextMeshProUGUI component not found on this GameObject.");
        }
    }
}