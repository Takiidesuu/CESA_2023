using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Deform;


public class DeformStage : MonoBehaviour
{
    private GameObject[] ChildMeshObject;                             //メッシュがあるオブジェクトを格納
    private Deformable[] ChildDefotmbles;
    private SAMeshColliderBuilder SAMeshColliderBuilder;

    private GameObject point_down;                                    //へこむポイントオブジェクト
    private  List<RadialCurveDeformer> all_point_down = new List<RadialCurveDeformer>(); //すべでのへこむポイントオブジェクト
    private List<GameObject> wave_deformer = new List<GameObject>();  //波
    private List<float> wave_instantiate_rotate_z = new List<float>();//波が生成されたときのプレイヤー角度
    private float wave_down_power = 0.5f;                             //波の減少
    private float wave_speed = 3;                                     //波の速さ

    private GameObject player_gameobject;                             //プレイヤー
    private GroundCheck ground_check;                                 //ステージの地面がどれかのチェック
    public bool hit_electrical;                                       //電源に当たっているか
    public Material electric_floor;                                  //電源に当たった際のマテリアル
    public Material floor;                                           //当たっていない際のマテリアル

    [SerializeField] private bool is_reverse;                         //全てを反転 （仮）

    public float point_down_factor;                                  //へこむ力

    void Start()
    {
        point_down = (GameObject)Resources.Load("PointDown");         //へこむオブジェクト取得
        player_gameobject = GameObject.FindWithTag("Player");
        ground_check = GameObject.Find("GroundCheck").GetComponent<GroundCheck>();

        //開始時にメッシュがあるオブジェクトを検索格納
        int meshcount = 0;
        for (int i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i).GetComponent<MeshRenderer>())
                meshcount++;
        ChildMeshObject = new GameObject[meshcount];
        ChildDefotmbles = new Deformable[meshcount];
        meshcount = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<MeshRenderer>())
            {
                ChildMeshObject[meshcount] = transform.GetChild(i).gameObject;
                meshcount++;
            }
        }

        //メッシュがあるものにDeformコンポーネントを追加
        for (int i = 0; i < ChildMeshObject.Length; i++)
        {
            if (!ChildMeshObject[i].GetComponent<Deformable>())
                ChildDefotmbles[i] = ChildMeshObject[i].AddComponent<Deformable>();
            else
                ChildDefotmbles[i] = ChildMeshObject[i].GetComponent<Deformable>();
        }

        //メッシュコライダー適用
        if (!gameObject.GetComponent<SAMeshColliderBuilder>())
            SAMeshColliderBuilder = gameObject.AddComponent<SAMeshColliderBuilder>();
        else
            SAMeshColliderBuilder = gameObject.GetComponent<SAMeshColliderBuilder>();
        SAMeshColliderBuilder.reducerProperty.shapeType = SAColliderBuilderCommon.ShapeType.Mesh;
        SAMeshColliderBuilder.reducerProperty.meshType = SAColliderBuilderCommon.MeshType.Raw;

        for (int i = 0; i < ChildMeshObject.Length; i++)
        {
            //ChildMeshObject[i].transform.GetChild(0).GetChild(0).gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            ChildDefotmbles[i].ColliderRecalculation = ColliderRecalculation.Auto;
            ChildDefotmbles[i].MeshCollider = ChildMeshObject[i].transform.GetChild(0).GetChild(0).gameObject.GetComponent<MeshCollider>();
        }
    }

    private void Update()
    {
        //波があるなら処理する
        if (wave_deformer.Count != 0) 
        {
            //一定の距離以上ならなら消す
            for (int i = 0; i < wave_deformer.Count; i += 2)
            {
                for (int j = 0; j < 2; j++)
                {
                    wave_deformer[i + j].GetComponent<RadialCurveDeformer>().Factor += wave_down_power;
                    if (wave_deformer[i + j].GetComponent<RadialCurveDeformer>().Factor > 0)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            Destroy(wave_deformer[i + j].gameObject);
                            wave_deformer.RemoveAt(i + j);
                            wave_instantiate_rotate_z.RemoveAt(i + j);

                            //デフォームを無ければ消す
                            foreach (Deformable deformable in ChildDefotmbles)
                            {
                                for (int l = 0; l < deformable.DeformerElements.Count; l++)
                                {
                                    if (deformable.DeformerElements[l].Component == null)
                                    {
                                        deformable.DeformerElements.RemoveAt(l);
                                        l = 0;
                                    }
                                }
                            }
                        }
                        break;
                    }
                }
            }

            //左右に移動させる（2個の配列を一セットとして0←,1→ にしている）
            for (int i = 0; i < wave_deformer.Count; i++)
            {
                if (i % 2 == 0)
                {
                    Vector3 temp = wave_deformer[i].transform.localPosition;
                    wave_deformer[i].transform.localPosition = new Vector3(temp.x + (wave_speed * Time.deltaTime * -Mathf.Sin(wave_instantiate_rotate_z[i] * Mathf.Deg2Rad)), temp.y + (wave_speed * Time.deltaTime * Mathf.Cos(wave_instantiate_rotate_z[i] * Mathf.Deg2Rad)), temp.z);
                }
                else
                {
                    Vector3 temp = wave_deformer[i].transform.localPosition;
                    wave_deformer[i].transform.localPosition = new Vector3(temp.x - (wave_speed * Time.deltaTime * -Mathf.Sin(wave_instantiate_rotate_z[i] * Mathf.Deg2Rad)), temp.y - (wave_speed * Time.deltaTime * Mathf.Cos(wave_instantiate_rotate_z[i] * Mathf.Deg2Rad)), temp.z);
                }
            }
        }
        //現在へこませているのを全て逆にする
        //if (is_reverse)
        //{
        //    foreach(GameObject gameObject in all_point_down)
        //    {
        //        gameObject.transform.eulerAngles =
        //            new Vector3(gameObject.transform.eulerAngles.x + 180,
        //                        gameObject.transform.eulerAngles.y,
        //                        gameObject.transform.eulerAngles.z);
        //    }

        //    is_reverse = false;
        //}
        //当たっている場合色を変化
        if (hit_electrical)
        {
            for (int i = 0; i < ChildMeshObject.Length; i++)
                ChildMeshObject[i].GetComponent<MeshRenderer>().material = electric_floor;
        }
        else
        {
            for (int i = 0; i < ChildMeshObject.Length; i++)
                ChildMeshObject[i].GetComponent<MeshRenderer>().material = floor;
        }

        //Debug.Log("中心とプレイヤーの角度"+ GetAngle(transform.position, player_gameobject.transform.position));
        //Debug.Log("プレイヤー角度" + player_gameobject.transform.eulerAngles.z);
    }

    //へこむオブジェクトを追加
    public void AddDeformpointDown(Vector3 position, float angleY, float smash_power,  bool isflip)
    {
        if (player_gameobject.GetComponent<PlayerMove>().GetGroundObj().name == "WallSwich")
        {
            player_gameobject.GetComponent<PlayerMove>().GetWallswich().WallMove();
            return;
        } 

        List<GameObject> pointdown = new List<GameObject>();

        //内側からか外側からを判断
        //if (isflip)
        //{
        //    if (10 > angleY && angleY > -10)    //プレイヤーの向きによってプラスかマイナスか判断
        //    {
        //        for (int i = 0; i < 3; i++)
        //            pointdown.Add(Instantiate(point_down, position, Quaternion.Euler(-90 + angleZ, -90, 90), this.transform));
        //    }
        //    else
        //    {
        //        for (int i = 0; i < 3; i++)
        //            pointdown.Add(Instantiate(point_down, position, Quaternion.Euler(-90 - angleZ, -90, 90), this.transform));
        //    }
        //}
        //else
        //{
        //    if (170 < angleY && angleY < 190)
        //    {
        //        for (int i = 0; i < 3; i++)
        //            pointdown.Add(Instantiate(point_down, position, Quaternion.Euler(-90 - angleZ, -90, 90), this.transform));
        //    }
        //    else
        //    {
        //        for (int i = 0; i < 3; i++)
        //            pointdown.Add(Instantiate(point_down, position, Quaternion.Euler(-90 + angleZ, -90, 90), this.transform));
        //    }
        //}

        ///垂直にへこます場合
        float offset = 1.5f;
        float x = Mathf.Cos((-90 - GetAngle(transform.position, player_gameobject.transform.position) + 180) * Mathf.Deg2Rad);
        float y = Mathf.Sin((-90 - GetAngle(transform.position, player_gameobject.transform.position) + 180) * Mathf.Deg2Rad);

        if (isflip)
        {
            if (10 > angleY && angleY > -10)    //プレイヤーの向きによってプラスかマイナスか判断
            {
                for (int i = 0; i < 3; i++)
                    pointdown.Add(Instantiate(point_down, position, Quaternion.Euler(-90 - GetAngle(transform.position, player_gameobject.transform.position) + 180, -90, 90), this.transform));
            }
            else
            {
                for (int i = 0; i < 3; i++)
                    pointdown.Add(Instantiate(point_down, position, Quaternion.Euler(-90 - GetAngle(transform.position, player_gameobject.transform.position) + 180, -90, 90), this.transform));
            }
        }
        else
        {
            if (170 < angleY && angleY < 190)
            {
                position.x -= y * offset;
                position.y += x * offset;
                for (int i = 0; i < 3; i++)
                    pointdown.Add(Instantiate(point_down, position, Quaternion.Euler(-90 - GetAngle(transform.position, player_gameobject.transform.position), -90, 90), this.transform));
            }
            else
            {
                position.x += y * offset;
                position.y -= x * offset;
                for (int i = 0; i < 3; i++)
                    pointdown.Add(Instantiate(point_down, position, Quaternion.Euler(-90 - GetAngle(transform.position, player_gameobject.transform.position), -90, 90), this.transform));
            }
        }

        //力によってへこむ量を変化させる
        pointdown[0].GetComponent<RadialCurveDeformer>().Factor = -smash_power * point_down_factor;
        pointdown[1].GetComponent<RadialCurveDeformer>().Factor = -smash_power * point_down_factor;
        pointdown[2].GetComponent<RadialCurveDeformer>().Factor = -smash_power * point_down_factor;

        //HitGroundに当たっているステージに対して変形を適用させる
        GameObject[] gameObjects = ground_check.GetHitGround();
        bool synthesis = false; //合成したか
        foreach (GameObject gameObject in gameObjects)
        {
            Deformable deformable = gameObject.transform.parent.parent.GetComponent<Deformable>();
            //同じ角度の物があればそれに合わせる
            foreach(DeformerElement deformerElement in deformable.DeformerElements)
            {
                if (deformerElement.Component == null)
                    continue;
                if (deformerElement.Component.transform.eulerAngles.y != pointdown[0].transform.eulerAngles.y)
                    continue;

                float error = Mathf.Abs(deformerElement.Component.transform.eulerAngles.x - pointdown[0].transform.eulerAngles.x);
                if (error < 0.5f)
                {
                     deformerElement.Component.GetComponent<RadialCurveDeformer>().Factor += pointdown[0].GetComponent<RadialCurveDeformer>().Factor;
                    synthesis = true;
                    break;
                }
            }
            //合成したならば追加しない
            if (!synthesis)
                deformable.AddDeformer(pointdown[0].GetComponent<RadialCurveDeformer>());
            else
                Destroy(pointdown[0]);
            deformable.AddDeformer(pointdown[1].GetComponent<RadialCurveDeformer>());
            deformable.AddDeformer(pointdown[2].GetComponent<RadialCurveDeformer>());
        }
        all_point_down.Add(pointdown[0].GetComponent<RadialCurveDeformer>());
        wave_deformer.Add(pointdown[1]);
        wave_deformer.Add(pointdown[2]);
        for (int i = 0; i < 2; i++)
            wave_instantiate_rotate_z.Add(player_gameobject.transform.eulerAngles.z + 270);

        //へこみを全て保管する
        ////all_point_down.Add(pointdown[0].GetComponent<RadialCurveDeformer>());
    }

    //電源に当たったか
    public void IsElectricalPower(bool hit)
    {
        hit_electrical = hit;
    }

    //2こオブジェクトの角度を求める
    float GetAngle(Vector2 start, Vector2 target)
    {
        Vector2 dt = target - start;
        float rad = Mathf.Atan2(dt.x, dt.y);
        float degree = rad * Mathf.Rad2Deg;

        if (degree < 0)
        {
            degree += 360;
        }

        return degree;
    }
}