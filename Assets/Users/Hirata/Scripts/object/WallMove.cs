using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMove : MonoBehaviour
{       
    private WallSwitch wallSwitch;            //switchのスクリプト

    public Vector3 movePosition;              //一度の移動量
    public int separationCount;               //何回に分けるか  
    public float oneMoveTime;                 //一度の移動時間
    public float untilReturnTime;             //戻るまでの時間 
    public float returnTime;                  //戻るの移動時間

    private Vector3 startPosition;            //初期位置
    private Vector3 velocity = Vector3.zero;  //移動量保存
    private int count;                        //現在回数
    private float untilTime;                  //最後に移動させてからの時間

    void Start()
    {
        wallSwitch = transform.parent.GetComponent<WallSwitch>();
        startPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

        //最後から押した時間が過ぎたら戻る
        if (untilReturnTime < untilTime) 
        {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, startPosition, ref velocity, returnTime);
            //戻る途中にカウントを更新
            Vector3 pos = transform.localPosition - startPosition;
            if (pos.x != 0)
                count = (int)(pos.x / (movePosition.x / separationCount));
            else if (pos.y != 0)
                count = (int)(pos.y / (movePosition.y / separationCount));
        }
        else //それ以外は回数ごとに更新
        {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, startPosition + (movePosition / separationCount * count), ref velocity, oneMoveTime);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            OnceWallMove();
        }

        untilTime += Time.deltaTime;
    }

    //スイッチを押したら移動
    public void OnceWallMove()
    {
        if (separationCount > count)
            count++;
        untilTime = 0;
    }
}
