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

    private SoundManager soundManager;

    private void Start()
    {
        collector = gameObject.GetComponent<LightBulbCollector>();
        soundManager = GetComponent<SoundManager>();
    }

    void Update()
    {
        //リザルト処理
        if(GameObject.FindObjectOfType<LightBulbCollector>().IsCleared())
        {
            StartCoroutine(ActiveResultWindow(WaitTime));
        }
    }

    IEnumerator ActiveResultWindow(float time)
    {
        yield return new WaitForSeconds(time);
        soundManager.PlaySoundEffect("Clear");
        ResultWindow.SetActive(true);
    }
}
