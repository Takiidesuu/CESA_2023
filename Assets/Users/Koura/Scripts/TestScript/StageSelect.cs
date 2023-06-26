using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public class StageSelect : MonoBehaviour
{
    //ステージを選択したかを判断するフラグ
    public bool select_move = false;
    private float speed = 5.0f;
    [SerializeField] Transform target;

    //次のシーン名を取得する変数
    public string next_scene;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SelectMove();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            select_move = true;
        }
    }

    public void SelectMove()
    {
        if (select_move)
        {
            FindObjectOfType<SceneController>().SceneChange(next_scene);
            select_move = false;
        }

    }

}
