using UnityEngine;
using System.Collections;
using TMPro;

public class BulbManager : MonoBehaviour
{
    public int Bulb_Active;
    public int Bulb_Num;

    public GameObject Text_Active_obj;
    public GameObject Text_Num_obj;

    public float WaitTime = 0.5f;

    private LightBulbCollector collector;

    private TextMeshProUGUI Text_Active;
    private TextMeshProUGUI Text_Num;

    public GameObject ResultWindow;

    private void Start()
    {
        collector = gameObject.GetComponent<LightBulbCollector>();    }

    void Update()
    {
        //ÉäÉUÉãÉgèàóù
        if(GameObject.FindObjectOfType<LightBulbCollector>().IsCleared() && GameObject.FindObjectOfType<LightBulbClearTrigger>() == null)
        {
            StartCoroutine(ActiveResultWindow(WaitTime));
        }
    }

    IEnumerator ActiveResultWindow(float time)
    {
        yield return new WaitForSeconds(time);
        ResultWindow.SetActive(true);
    }
}
