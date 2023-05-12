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

        Text_Active = Text_Active_obj.GetComponent<TextMeshProUGUI>();
        Text_Num = Text_Num_obj.GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        Bulb_Active = collector.LightBulb_active;
        Bulb_Num = collector.LightBulb_num;

        Text_Active.text = "" + Bulb_Active.ToString();
        Text_Num.text = "" + Bulb_Num.ToString();

        //ÉäÉUÉãÉgèàóù
        if(Bulb_Active == Bulb_Num)
        {
            ResultWindow.SetActive(true);
        }
    }
}
