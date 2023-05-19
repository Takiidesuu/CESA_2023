using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricalPower : MonoBehaviour
{
    private DeformStage deformstage;    //当たったステージ
    public bool is_stage_hit = true;   //ステージに当たっているか
    private bool is_init = false;
    private int nothit_count;           //当たらずに何フレーム立ったか (Exitが呼ばれないため)
    public float electricball_instan_time = 5;//電球を繰り返し生成する時間
    public int electricball_max = 3;    //発射数の最大
    public float start_time;    //開始ラグ
    public float ElectricBallSpeed = 25;
    public GameObject ElectricBall;    //電球
    public List<GameObject> InstantElectricBall; //生成した電球配列
    private float hit_elapsed_time;    //継続ヒット時間
    private float time;                 //前回からの秒数
    private float old_time;             //前回生成した秒数
    private Vector3 electricball_position;//電球を生成する位置
    private ParticleSystem ChargeEffect;
    private GameObject ElectricBallEffect;

    private ElectricBallCounter ElectricBallCounter;

    private float RateOverTime;
    private float SimurationSpeed;
    private Vector3 ElectricBallScale;

    private void Start()
    {
        //エフェクトの格納
        ChargeEffect = gameObject.transform.parent.Find("ChargeEffect").GetComponent<ParticleSystem>();
        RateOverTime = ChargeEffect.emissionRate;
        SimurationSpeed = ChargeEffect.playbackSpeed;
        ElectricBallEffect = gameObject.transform.parent.Find("ElectricBall").gameObject;
        ElectricBallScale = ElectricBallEffect.transform.localScale;
        //配列の初期化
        InstantElectricBall = new List<GameObject>();
        ElectricBallCounter = GameObject.Find("Canvas").GetComponent<ElectricBallCounter>();
    }

    private void Update()
    {
        // リストの中身を検査する
        for (int i = InstantElectricBall.Count - 1; i >= 0; i--)
        {
            if (InstantElectricBall[i] == null) // オブジェクトがnullの場合、Missingとなったオブジェクトと見なす
            {
                InstantElectricBall.RemoveAt(i); // リストから削除する
            }
        }

        //生成数がMAXの場合Updateを回さない
        if (InstantElectricBall.Count >= electricball_max)
        {
            ChargeEffect.gameObject.active = false;
            Vector3 ballsize = ElectricBallScale;

            ballsize.x = 0;
            ballsize.y = 0;
            ballsize.z = 0;

            ElectricBallEffect.transform.localScale = ballsize;

            return;
        }
        ChargeEffect.gameObject.active = true;
        time = Time.time - old_time;
        hit_elapsed_time += Time.deltaTime;
        //deformstage.IsElectricalPower(true);                                //電源が当たった事をステージに渡す
        is_stage_hit = true;
        nothit_count = 0;

        //ステージとの接触が無くなった場合タイマーをリセット
        if (is_stage_hit)
        {

            if (!is_init)
            {
                old_time = Time.time;
                time = Time.time - old_time;
                is_init = true;
            }

            //エフェクトを変更
            ChargeEffect.playbackSpeed = (SimurationSpeed / electricball_instan_time) * time;
            ChargeEffect.emissionRate = (RateOverTime / electricball_instan_time) * time;

            Vector3 ballsize = ElectricBallScale;
            ballsize.x = (ballsize.x / electricball_instan_time) * time;
            ballsize.y = (ballsize.y / electricball_instan_time) * time;
            ballsize.z = (ballsize.z / electricball_instan_time) * time;

            ElectricBallEffect.transform.localScale = ballsize;


            if (time > electricball_instan_time - 1.5 && hit_elapsed_time > start_time)
            {
                ChargeEffect.Stop();
            }
            if (time > electricball_instan_time && hit_elapsed_time > start_time)
            {
                //SEの再生
                transform.GetComponent<SoundManager>().PlaySoundEffect("Create");
                GameObject ElectricBall_Instant = Instantiate(ElectricBall, transform.position, Quaternion.identity);
                ElectricBall_Instant.GetComponent<ElectricBallMove>().ParentGenerator = this.gameObject;
                ElectricBall_Instant.transform.GetComponent<Rigidbody>().AddForce(new Vector3(0, 10, 0));
                ElectricBall_Instant.GetComponent<ElectricBallMove>().ChangeRealSpeed(ElectricBallSpeed);
                //配列に生成した雷球を格納
                InstantElectricBall.Add(ElectricBall_Instant);
                old_time = Time.time;
                ChargeEffect.Clear();
                ChargeEffect.Play();
                is_init = false;
                ElectricBallCounter.CountUpBulb();
            }
        }
        else
        {
            hit_elapsed_time = 0;
            is_init = false;
            ChargeEffect.Stop();
        }
    }
}
