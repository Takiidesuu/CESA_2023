using UnityEngine;
using TMPro;

public class BulbManager : MonoBehaviour
{
    public int Bulb_Active;
    public int Bulb_Num;

    public GameObject Text_Active_obj;
    public GameObject Text_Num_obj;

    private LightBulbCollector collector;

    private TextMeshProUGUI Text_Active;
    private TextMeshProUGUI Text_Num;

    public GameObject ResultWindow;

    private void Start()
    {
        collector = gameObject.GetComponent<LightBulbCollector>();
    }

    void Update()
    {
        //ÉäÉUÉãÉgèàóù
        if(GameObject.FindObjectOfType<LightBulbCollector>().IsCleared())
        {
            ResultWindow.SetActive(true);
        }
    }
}
